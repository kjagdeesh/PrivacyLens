using PrivacyLens.Data;
using PrivacyLens.Data.Entities;
using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services
{
    public class DataSyncService : IDataSyncService
    {
        private readonly PrivacyLensDatabase _database;
        private readonly IInstalledAppsService _appsService;
        private readonly IPermissionService _permissionService;
        private readonly IPermissionUsageService _usageService;
        private readonly IDeviceCapabilityService _capabilityService;

        private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

        public DataSyncService(
            PrivacyLensDatabase database,
            IInstalledAppsService appsService,
            IPermissionService permissionService,
            IPermissionUsageService usageService,
            IDeviceCapabilityService capabilityService)
        {
            _database = database;
            _appsService = appsService;
            _permissionService = permissionService;
            _usageService = usageService;
            _capabilityService = capabilityService;
        }

        public async Task<bool> IsRefreshRequiredAsync(CancellationToken cancellationToken = default)
        {
            var db = await _database.GetDatabaseAsync();
            var metadata = await db.Table<SyncMetadataEntity>()
                                   .Where(m => m.SyncType == SyncType.FullDeviceScan)
                                   .FirstOrDefaultAsync();

            if (metadata == null || metadata.LastSuccessfulSyncAt == null)
            {
                return true;
            }

            var age = DateTime.UtcNow - metadata.LastSuccessfulSyncAt.Value;
            return age >= TimeSpan.FromHours(3);
        }

        public async Task SyncAsync(bool forceRefresh, CancellationToken cancellationToken = default)
        {
            await _syncSemaphore.WaitAsync(cancellationToken);
            try
            {
                var db = await _database.GetDatabaseAsync();
                System.Diagnostics.Debug.WriteLine("[DataSyncService] DB obtained.");

                if (!forceRefresh)
                {
                    var isStale = await IsRefreshRequiredAsync(cancellationToken);
                    if (!isStale)
                    {
                        System.Diagnostics.Debug.WriteLine("[DataSyncService] Cache is fresh, skipping sync.");
                        return;
                    }
                }

                var metadata = await db.Table<SyncMetadataEntity>()
                                       .Where(m => m.SyncType == SyncType.FullDeviceScan)
                                       .FirstOrDefaultAsync();

                if (metadata == null)
                {
                    metadata = new SyncMetadataEntity
                    {
                        SyncType = SyncType.FullDeviceScan,
                        SyncStatus = SyncStatus.NeverRun
                    };
                    await db.InsertAsync(metadata);
                }

                metadata.LastAttemptedSyncAt = DateTime.UtcNow;
                metadata.SyncStatus = SyncStatus.Running;
                await db.UpdateAsync(metadata);

                try
                {
                    // ── 1. Fetch all data OUTSIDE the synchronous transaction ──────────────
                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Fetching installed apps...");
                    var apps = (await _appsService.GetInstalledAppsAsync(cancellationToken)).ToList();
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] Got {apps.Count} apps from scanner.");
                    var uniqueApps = apps.GroupBy(a => a.PackageName).Select(g => g.First()).ToList();
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] {uniqueApps.Count} unique apps after dedup.");

                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Fetching per-app permissions...");
                    var allAppPermissions = new Dictionary<string, List<AppPermission>>();
                    foreach (var app in uniqueApps)
                    {
                        if (cancellationToken.IsCancellationRequested) break;
                        try
                        {
                            var perms = (await _permissionService.GetPermissionsForAppAsync(app.PackageName, cancellationToken))
                                .GroupBy(p => p.PermissionName).Select(g => g.First()).ToList();
                            allAppPermissions[app.PackageName] = perms;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DataSyncService] Failed perms for {app.PackageName}: {ex.Message}");
                            allAppPermissions[app.PackageName] = new List<AppPermission>();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Fetching device permissions...");
                    var devPerms = (await _permissionService.GetSupportedPermissionsAsync(cancellationToken)).ToList();
                    var uniqueDevPerms = devPerms.GroupBy(p => p.PermissionName).Select(g => g.First()).ToList();
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] Got {uniqueDevPerms.Count} device permissions.");

                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Fetching today's usage records...");
                    var usageRecords = (await _usageService.GetTodayUsageAsync(cancellationToken)).ToList();
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] Got {usageRecords.Count} usage records.");

                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Fetching device capabilities...");
                    var capabilities = (await _capabilityService.GetPermissionCapabilitiesAsync(cancellationToken)).ToList();
                    var uniqueCapabilities = capabilities.GroupBy(c => c.Category).Select(g => g.First()).ToList();
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] Got {uniqueCapabilities.Count} capabilities.");

                    // ── 2. Write to DB inside one synchronous transaction ─────────────────
                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Starting DB transaction...");
                    await db.RunInTransactionAsync(conn =>
                    {
                        // Apps
                        conn.DeleteAll<InstalledAppEntity>();
                        foreach (var app in uniqueApps)
                        {
                            conn.Insert(new InstalledAppEntity
                            {
                                PackageName = app.PackageName,
                                Name = app.Name,
                                VersionName = app.VersionName,
                                VersionCode = app.VersionCode,
                                IsSystemApp = app.IsSystemApp,
                                AppProvidedDescription = app.AppProvidedDescription,
                                IconCachePath = app.IconCachePath,
                                GrantedSensitivePermissionCount = app.GrantedSensitivePermissionCount,
                                FirstDetectedAt = DateTime.UtcNow,
                                LastDetectedAt = DateTime.UtcNow,
                                LastUpdatedAt = DateTime.UtcNow
                            });

                            // App permissions
                            if (allAppPermissions.TryGetValue(app.PackageName, out var appPerms))
                            {
                                conn.Execute("DELETE FROM AppPermissions WHERE PackageName = ?", app.PackageName);
                                
                                // Find the latest usage for this app from the fetched usage records
                                var appUsage = usageRecords.Where(u => u.PackageName == app.PackageName)
                                                           .OrderByDescending(u => u.AccessTime)
                                                           .FirstOrDefault();

                                foreach (var perm in appPerms)
                                {
                                    var lastAccess = perm.LastAccessTime?.UtcDateTime;
                                    var availability = perm.UsageDataAvailability;

                                    // If we got a valid app usage time, use it as a proxy for permission usage
                                    if (appUsage?.AccessTime != null)
                                    {
                                        lastAccess = appUsage.AccessTime.Value.UtcDateTime;
                                        availability = DataAvailability.Available;
                                    }

                                    conn.Insert(new AppPermissionEntity
                                    {
                                        PackageName = app.PackageName,
                                        PermissionName = perm.PermissionName,
                                        DisplayName = perm.DisplayName,
                                        PermissionCategory = perm.Category,
                                        PermissionAccessStatus = perm.Status,
                                        IsGranted = perm.IsGranted,
                                        LastAccessTime = lastAccess,
                                        UsageDataAvailability = availability,
                                        LastUpdatedAt = DateTime.UtcNow
                                    });
                                }
                            }
                        }
                        System.Diagnostics.Debug.WriteLine($"[DataSyncService] Inserted {uniqueApps.Count} apps into DB.");

                        // Device-level permissions
                        conn.DeleteAll<DevicePermissionEntity>();
                        foreach (var devPerm in uniqueDevPerms)
                        {
                            var grantedCount = conn.ExecuteScalar<int>(
                                "SELECT COUNT(DISTINCT PackageName) FROM AppPermissions WHERE PermissionCategory = ? AND IsGranted = 1",
                                (int)devPerm.Category);
                            conn.Insert(new DevicePermissionEntity
                            {
                                PermissionName = devPerm.PermissionName,
                                DisplayName = devPerm.DisplayName,
                                Description = devPerm.Description,
                                PermissionCategory = devPerm.Category,
                                GrantedAppCount = grantedCount,
                                IsSupported = devPerm.IsSupported,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                        }

                        // Usage records
                        conn.DeleteAll<PermissionUsageEntity>();
                        foreach (var usage in usageRecords)
                        {
                            conn.Insert(new PermissionUsageEntity
                            {
                                PackageName = usage.PackageName,
                                AppName = usage.AppName,
                                PermissionCategory = usage.Category,
                                PermissionName = usage.PermissionName,
                                AccessTime = usage.AccessTime?.UtcDateTime,
                                IsBackgroundAccess = usage.IsBackgroundAccess,
                                IsCurrentlyActive = usage.IsCurrentlyActive,
                                DataAvailability = usage.DataAvailability,
                                DetectedAt = DateTime.UtcNow
                            });
                        }

                        // Capabilities
                        conn.DeleteAll<PermissionCapabilityEntity>();
                        foreach (var cap in uniqueCapabilities)
                        {
                            conn.Insert(new PermissionCapabilityEntity
                            {
                                PermissionCategory = cap.Category,
                                IsSupported = cap.IsSupported,
                                CanReadGrantedStatus = cap.CanReadGrantedStatus,
                                CanReadLastUsage = cap.CanReadLastUsage,
                                RequiresSpecialAccess = cap.RequiresSpecialAccess,
                                LimitationDescription = cap.LimitationDescription,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                        }

                        // Cleanup stale records
                        var activePackageNames = uniqueApps.Select(a => a.PackageName).ToList();
                        PerformDatabaseCleanup(conn, activePackageNames);
                        System.Diagnostics.Debug.WriteLine("[DataSyncService] Transaction committed.");
                    });

                    metadata.LastSuccessfulSyncAt = DateTime.UtcNow;
                    metadata.SyncStatus = SyncStatus.Completed;
                    metadata.ErrorMessage = null;
                    await db.UpdateAsync(metadata);
                    System.Diagnostics.Debug.WriteLine("[DataSyncService] Sync completed successfully.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DataSyncService] SYNC FAILED: {ex}");
                    metadata.SyncStatus = SyncStatus.Failed;
                    metadata.ErrorMessage = ex.Message;
                    await db.UpdateAsync(metadata);
                    throw;
                }
            }
            finally
            {
                _syncSemaphore.Release();
            }
        }

        private void PerformDatabaseCleanup(SQLiteConnection conn, List<string> activePackageNames)
        {
            try
            {
                var appsToDelete = conn.Table<InstalledAppEntity>()
                                       .ToList()
                                       .Where(a => !activePackageNames.Contains(a.PackageName))
                                       .ToList();

                foreach (var app in appsToDelete)
                {
                    if (!string.IsNullOrEmpty(app.IconCachePath) && File.Exists(app.IconCachePath))
                    {
                        try { File.Delete(app.IconCachePath); }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to delete icon file {app.IconCachePath}: {ex}");
                        }
                    }

                    conn.Execute("DELETE FROM AppPermissions WHERE PackageName = ?", app.PackageName);
                    conn.Execute("DELETE FROM PermissionUsages WHERE PackageName = ?", app.PackageName);
                    conn.Delete(app);
                }

                var thresholdDate = DateTime.UtcNow.AddDays(-30);
                conn.Execute("DELETE FROM PermissionUsages WHERE AccessTime < ?", thresholdDate);
                conn.Execute("DELETE FROM AppPermissions WHERE PackageName NOT IN (SELECT PackageName FROM InstalledApps)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during database cleanup: {ex}");
            }
        }
    }
}
