using Android.App.Usage;
using Android.Content;
using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Platforms.Android.Services
{
    public class AndroidPermissionUsageService : IPermissionUsageService
    {
        public Task<IEnumerable<PermissionUsageRecord>> GetTodayUsageAsync(CancellationToken cancellationToken = default)
        {
            var records = new List<PermissionUsageRecord>();
            try
            {
                var context = global::Android.App.Application.Context;
                var usageStatsManager = (UsageStatsManager?)context.GetSystemService(Context.UsageStatsService);
                
                if (usageStatsManager != null)
                {
                    var cal = Java.Util.Calendar.GetInstance(Java.Util.TimeZone.Default);
                    if (cal != null)
                    {
                        cal.Add(Java.Util.CalendarField.DayOfYear, -1);
                        long startTime = cal.TimeInMillis;
                        long endTime = Java.Lang.JavaSystem.CurrentTimeMillis();

                        var stats = usageStatsManager.QueryUsageStats(UsageStatsInterval.Daily, startTime, endTime);

                        if (stats != null && stats.Count > 0)
                        {
                            var packageManager = context.PackageManager;
                            
                            var recentStats = stats.Where(s => s.LastTimeUsed > 0)
                                                   .GroupBy(s => s.PackageName)
                                                   .Select(g => g.OrderByDescending(s => s.LastTimeUsed).First())
                                                   .OrderByDescending(s => s.LastTimeUsed)
                                                   .Take(50);

                            foreach (var stat in recentStats)
                            {
                                if (string.IsNullOrEmpty(stat.PackageName)) continue;
                                
                                string appName = stat.PackageName;
                                try
                                {
                                    var appInfo = packageManager?.GetApplicationInfo(stat.PackageName, 0);
                                    if (appInfo != null)
                                    {
                                        appName = packageManager?.GetApplicationLabel(appInfo) ?? stat.PackageName;
                                    }
                                }
                                catch { }

                                records.Add(new PermissionUsageRecord
                                {
                                    PackageName = stat.PackageName,
                                    AppName = appName,
                                    Category = PermissionCategory.Unknown,
                                    PermissionName = "App Usage",
                                    AccessTime = DateTimeOffset.FromUnixTimeMilliseconds(stat.LastTimeUsed),
                                    DataAvailability = DataAvailability.Available
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPermissionUsageService] Error: {ex}");
            }

            return Task.FromResult<IEnumerable<PermissionUsageRecord>>(records);
        }

        public Task<PermissionUsageRecord?> GetLastUsageAsync(string packageName, PermissionCategory category, CancellationToken cancellationToken = default)
        {
            try
            {
                var context = global::Android.App.Application.Context;
                var usageStatsManager = (UsageStatsManager?)context.GetSystemService(Context.UsageStatsService);

                if (usageStatsManager != null)
                {
                    var cal = Java.Util.Calendar.GetInstance(Java.Util.TimeZone.Default);
                    if (cal != null)
                    {
                        cal.Add(Java.Util.CalendarField.DayOfYear, -7);
                        long startTime = cal.TimeInMillis;
                        long endTime = Java.Lang.JavaSystem.CurrentTimeMillis();

                        var stats = usageStatsManager.QueryUsageStats(UsageStatsInterval.Daily, startTime, endTime);

                        if (stats != null)
                        {
                            var packageStats = stats.Where(s => s.PackageName == packageName && s.LastTimeUsed > 0)
                                                    .OrderByDescending(s => s.LastTimeUsed)
                                                    .FirstOrDefault();

                            if (packageStats != null)
                            {
                                return Task.FromResult<PermissionUsageRecord?>(new PermissionUsageRecord
                                {
                                    PackageName = packageName,
                                    AppName = packageName,
                                    Category = category,
                                    PermissionName = "App Usage",
                                    AccessTime = DateTimeOffset.FromUnixTimeMilliseconds(packageStats.LastTimeUsed),
                                    DataAvailability = DataAvailability.Available
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPermissionUsageService] Error: {ex}");
            }

            return Task.FromResult<PermissionUsageRecord?>(new PermissionUsageRecord
            {
                PackageName = packageName,
                Category = category,
                DataAvailability = DataAvailability.RestrictedByAndroid
            });
        }

        public bool HasUsageAccess()
        {
            try
            {
                var context = global::Android.App.Application.Context;
                var appOps = (global::Android.App.AppOpsManager?)context.GetSystemService(Context.AppOpsService);
                if (appOps != null)
                {
                    var mode = appOps.CheckOpNoThrow(global::Android.App.AppOpsManager.OpstrGetUsageStats, global::Android.OS.Process.MyUid(), context.PackageName);
                    return mode == global::Android.App.AppOpsManagerMode.Allowed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPermissionUsageService] Error checking usage access: {ex}");
            }
            return false;
        }

        public void RequestUsageAccess()
        {
            try
            {
                var context = global::Android.App.Application.Context;
                var intent = new Intent(global::Android.Provider.Settings.ActionUsageAccessSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                
                try
                {
                    // Attempt to highlight our app directly if supported
                    var uri = global::Android.Net.Uri.FromParts("package", context.PackageName, null);
                    intent.SetData(uri);
                    context.StartActivity(intent);
                }
                catch
                {
                    // Fallback to the main usage access page if the direct package URI fails
                    intent.SetData(null);
                    context.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPermissionUsageService] Error requesting usage access: {ex}");
            }
        }
    }
}
