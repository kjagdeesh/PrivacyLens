using PrivacyLens.Models;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#if ANDROID
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
#endif

namespace PrivacyLens.Platforms.Android.Services
{
    public class AndroidInstalledAppsService : IInstalledAppsService
    {
        public Task<IEnumerable<InstalledApp>> GetInstalledAppsAsync(CancellationToken cancellationToken = default)
        {
            var apps = new List<InstalledApp>();
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var packageManager = context.PackageManager;
                if (packageManager == null) return Task.FromResult<IEnumerable<InstalledApp>>(apps);

                // Fetch basic packages (flag = 0) to avoid TransactionTooLargeException on devices with many apps
                var packages = packageManager.GetInstalledPackages((PackageInfoFlags)0);
                if (packages == null) return Task.FromResult<IEnumerable<InstalledApp>>(apps);
                
                var cacheDir = System.IO.Path.Combine(FileSystem.AppDataDirectory, "app-icons");
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }

                foreach (var packageInfo in packages)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    if (packageInfo.ApplicationInfo == null) continue;

                    var packageName = packageInfo.PackageName;
                    var appName = packageInfo.ApplicationInfo.LoadLabel(packageManager) ?? packageName;
                    var versionName = packageInfo.VersionName;
                    var versionCode = packageInfo.LongVersionCode;
                    var isSystem = (packageInfo.ApplicationInfo.Flags & ApplicationInfoFlags.System) != 0;
                    
                    string? appDescription = null;
                    try
                    {
                        var descRes = packageInfo.ApplicationInfo.DescriptionRes;
                        if (descRes != 0)
                        {
                            appDescription = packageManager.GetText(packageName, descRes, packageInfo.ApplicationInfo)?.ToString();
                        }
                        
                        if (string.IsNullOrWhiteSpace(appDescription))
                        {
                            var loadedDesc = packageInfo.ApplicationInfo.LoadDescription(packageManager)?.ToString();
                            if (!string.IsNullOrWhiteSpace(loadedDesc))
                            {
                                appDescription = loadedDesc;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore description load errors
                    }

                    string? iconCachePath = null;
                    try
                    {
                        var iconDrawable = packageInfo.ApplicationInfo.LoadIcon(packageManager);
                        if (iconDrawable != null)
                        {
                            iconCachePath = System.IO.Path.Combine(cacheDir, $"{packageName}.png");
                            if (!File.Exists(iconCachePath))
                            {
                                SaveIconToFile(iconDrawable, iconCachePath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to cache icon for {packageName}: {ex}");
                        iconCachePath = null;
                    }

                    int sensitiveCount = 0;
                    try
                    {
                        var packageInfoWithPerms = packageManager.GetPackageInfo(packageName, PackageInfoFlags.Permissions);
                        if (packageInfoWithPerms?.RequestedPermissions != null)
                        {
                            for (int i = 0; i < packageInfoWithPerms.RequestedPermissions.Count; i++)
                            {
                                var permissionName = packageInfoWithPerms.RequestedPermissions[i];
                                var category = Helpers.PermissionMapper.MapPermissionToCategory(permissionName);
                                if (category != Enums.PermissionCategory.Unknown)
                                {
                                    bool isGranted = packageManager.CheckPermission(permissionName, packageName) == Permission.Granted;
                                    if (isGranted)
                                    {
                                        sensitiveCount++;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore permission queries for individual apps if they fail
                    }

                    apps.Add(new InstalledApp
                    {
                        PackageName = packageName,
                        Name = appName,
                        VersionName = versionName,
                        VersionCode = versionCode,
                        IsSystemApp = isSystem,
                        AppProvidedDescription = appDescription,
                        IconCachePath = iconCachePath,
                        GrantedSensitivePermissionCount = sensitiveCount
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scanning installed apps: {ex}");
            }
#endif
            return Task.FromResult<IEnumerable<InstalledApp>>(apps);
        }

        public Task<InstalledApp?> GetAppAsync(string packageName, CancellationToken cancellationToken = default)
        {
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var packageManager = context.PackageManager;
                if (packageManager == null) return Task.FromResult<InstalledApp?>(null);

                var packageInfo = packageManager.GetPackageInfo(packageName, PackageInfoFlags.Permissions);
                if (packageInfo == null || packageInfo.ApplicationInfo == null) return Task.FromResult<InstalledApp?>(null);

                var cacheDir = System.IO.Path.Combine(FileSystem.AppDataDirectory, "app-icons");
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }

                var appName = packageInfo.ApplicationInfo.LoadLabel(packageManager) ?? packageName;
                var versionName = packageInfo.VersionName;
                var versionCode = packageInfo.LongVersionCode;
                var isSystem = (packageInfo.ApplicationInfo.Flags & ApplicationInfoFlags.System) != 0;

                string? appDescription = null;
                try
                {
                    var descRes = packageInfo.ApplicationInfo.DescriptionRes;
                    if (descRes != 0)
                    {
                        appDescription = packageManager.GetText(packageName, descRes, packageInfo.ApplicationInfo)?.ToString();
                    }
                    
                    if (string.IsNullOrWhiteSpace(appDescription))
                    {
                        var loadedDesc = packageInfo.ApplicationInfo.LoadDescription(packageManager)?.ToString();
                        if (!string.IsNullOrWhiteSpace(loadedDesc))
                        {
                            appDescription = loadedDesc;
                        }
                    }
                }
                catch
                {
                    // Ignore description load errors
                }

                string? iconCachePath = null;
                try
                {
                    var iconDrawable = packageInfo.ApplicationInfo.LoadIcon(packageManager);
                    if (iconDrawable != null)
                    {
                        iconCachePath = System.IO.Path.Combine(cacheDir, $"{packageName}.png");
                        if (!File.Exists(iconCachePath))
                        {
                            SaveIconToFile(iconDrawable, iconCachePath);
                        }
                    }
                }
                catch
                {
                    iconCachePath = null;
                }

                int sensitiveCount = 0;
                if (packageInfo.RequestedPermissions != null)
                {
                    for (int i = 0; i < packageInfo.RequestedPermissions.Count; i++)
                    {
                        var permissionName = packageInfo.RequestedPermissions[i];
                        var category = Helpers.PermissionMapper.MapPermissionToCategory(permissionName);
                        if (category != Enums.PermissionCategory.Unknown)
                        {
                            bool isGranted = packageManager.CheckPermission(permissionName, packageName) == Permission.Granted;
                            if (isGranted)
                            {
                                sensitiveCount++;
                            }
                        }
                    }
                }

                return Task.FromResult<InstalledApp?>(new InstalledApp
                {
                    PackageName = packageName,
                    Name = appName,
                    VersionName = versionName,
                    VersionCode = versionCode,
                    IsSystemApp = isSystem,
                    AppProvidedDescription = appDescription,
                    IconCachePath = iconCachePath,
                    GrantedSensitivePermissionCount = sensitiveCount
                });
            }
            catch
            {
                return Task.FromResult<InstalledApp?>(null);
            }
#else
            return Task.FromResult<InstalledApp?>(null);
#endif
        }

#if ANDROID
        private void SaveIconToFile(Drawable drawable, string filePath)
        {
            Bitmap? bitmap = null;

            if (drawable is BitmapDrawable bitmapDrawable && bitmapDrawable.Bitmap != null)
            {
                bitmap = bitmapDrawable.Bitmap;
            }
            else
            {
                int width = drawable.IntrinsicWidth > 0 ? drawable.IntrinsicWidth : 72;
                int height = drawable.IntrinsicHeight > 0 ? drawable.IntrinsicHeight : 72;

                bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!);
                var canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
            }

            using (var stream = File.OpenWrite(filePath))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);
            }
        }
#endif
    }
}
