using PrivacyLens.Data;
using PrivacyLens.Data.Entities;
using PrivacyLens.Data.Mappers;
using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Repositories.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyLens.Repositories
{
    public class PrivacyRepository : IPrivacyRepository
    {
        private readonly PrivacyLensDatabase _database;

        public PrivacyRepository(PrivacyLensDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<InstalledApp>> GetAppsAsync()
        {
            var db = await _database.GetDatabaseAsync();
            var entities = await db.Table<InstalledAppEntity>()
                                   .OrderBy(a => a.Name)
                                   .ToListAsync();
            return entities.Select(e => e.ToDomain());
        }

        public async Task<InstalledApp?> GetAppAsync(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) return null;

            var db = await _database.GetDatabaseAsync();
            var entity = await db.Table<InstalledAppEntity>()
                                 .FirstOrDefaultAsync(a => a.PackageName == packageName);
            return entity?.ToDomain();
        }

        public async Task<IEnumerable<AppPermission>> GetAppPermissionsAsync(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) return Enumerable.Empty<AppPermission>();

            var db = await _database.GetDatabaseAsync();
            var entities = await db.Table<AppPermissionEntity>()
                                   .Where(p => p.PackageName == packageName)
                                   .ToListAsync();
            return entities.Select(e => e.ToDomain());
        }

        public async Task<IEnumerable<DevicePermission>> GetPermissionsAsync()
        {
            var db = await _database.GetDatabaseAsync();
            var entities = await db.Table<DevicePermissionEntity>()
                                   .OrderBy(p => p.PermissionName)
                                   .ToListAsync();
            return entities.Select(e => e.ToDomain());
        }

        public async Task<IEnumerable<InstalledApp>> GetAppsForPermissionAsync(PermissionCategory category)
        {
            var db = await _database.GetDatabaseAsync();
            var allowedPerms = await db.Table<AppPermissionEntity>()
                                       .Where(p => p.PermissionCategory == category && p.IsGranted)
                                       .ToListAsync();

            if (!allowedPerms.Any()) return Enumerable.Empty<InstalledApp>();

            var packageNames = allowedPerms.Select(p => p.PackageName).Distinct().ToList();
            
            var apps = await db.Table<InstalledAppEntity>()
                               .Where(a => packageNames.Contains(a.PackageName))
                               .ToListAsync();

            return apps.Select(e => e.ToDomain());
        }

        public async Task<List<PermissionAccessItem>> GetPermissionAccessItemsAsync(PermissionCategory category)
        {
            var db = await _database.GetDatabaseAsync();
            
            var allowedPerms = await db.Table<AppPermissionEntity>()
                                       .Where(p => p.PermissionCategory == category && p.IsGranted)
                                       .ToListAsync();

            if (!allowedPerms.Any()) return new List<PermissionAccessItem>();

            var packageNames = allowedPerms.Select(p => p.PackageName).Distinct().ToList();
            
            var appsList = await db.Table<InstalledAppEntity>()
                                   .Where(a => packageNames.Contains(a.PackageName))
                                   .ToListAsync();

            var appsDict = appsList.ToDictionary(a => a.PackageName, a => a);

            var items = new List<PermissionAccessItem>();
            var groupedByApp = allowedPerms.GroupBy(p => p.PackageName);

            foreach (var group in groupedByApp)
            {
                if (!appsDict.TryGetValue(group.Key, out var app)) continue;

                var primaryPerm = group.OrderByDescending(p => p.PermissionAccessStatus).First();
                var lastAccess = group.Max(p => p.LastAccessTime);

                items.Add(new PermissionAccessItem
                {
                    PackageName = app.PackageName,
                    AppName = app.Name,
                    IconCachePath = app.IconCachePath,
                    Category = category,
                    PermissionName = string.IsNullOrEmpty(primaryPerm.DisplayName) ? primaryPerm.PermissionName : primaryPerm.DisplayName,
                    Status = primaryPerm.PermissionAccessStatus,
                    IsGranted = true,
                    LastAccessTime = lastAccess.HasValue ? new DateTimeOffset(lastAccess.Value, TimeSpan.Zero) : null,
                    UsageDataAvailability = primaryPerm.UsageDataAvailability
                });
            }

            return items.OrderByDescending(i => i.LastAccessTime.HasValue)
                        .ThenByDescending(i => i.LastAccessTime)
                        .ThenBy(i => i.AppName)
                        .ToList();
        }

        public async Task<IEnumerable<PermissionUsageRecord>> GetTodayPermissionActivityAsync()
        {
            var db = await _database.GetDatabaseAsync();
            var today = DateTime.UtcNow.Date;
            
            var entities = await db.Table<PermissionUsageEntity>()
                                   .Where(u => u.AccessTime >= today)
                                   .OrderByDescending(u => u.AccessTime)
                                   .ToListAsync();

            return entities.Select(e => e.ToDomain());
        }

        public async Task<DateTimeOffset?> GetLastSuccessfulSyncTimeAsync()
        {
            var db = await _database.GetDatabaseAsync();
            var metadata = await db.Table<SyncMetadataEntity>()
                                   .Where(m => m.SyncType == SyncType.FullDeviceScan)
                                   .FirstOrDefaultAsync();

            if (metadata?.LastSuccessfulSyncAt == null) return null;
            
            return new DateTimeOffset(metadata.LastSuccessfulSyncAt.Value, TimeSpan.Zero);
        }
    }
}
