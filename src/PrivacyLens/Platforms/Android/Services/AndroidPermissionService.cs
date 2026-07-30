using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if ANDROID
using Android.Content.PM;
#endif

namespace PrivacyLens.Platforms.Android.Services
{
    public class AndroidPermissionService : IPermissionService
    {
        public Task<IEnumerable<AppPermission>> GetPermissionsForAppAsync(string packageName, CancellationToken cancellationToken = default)
        {
            var permissions = new List<AppPermission>();
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var packageManager = context.PackageManager;
                if (packageManager == null) return Task.FromResult<IEnumerable<AppPermission>>(permissions);

                var packageInfo = packageManager.GetPackageInfo(packageName, PackageInfoFlags.Permissions);
                if (packageInfo == null || packageInfo.RequestedPermissions == null)
                {
                    return Task.FromResult<IEnumerable<AppPermission>>(permissions);
                }

                bool hasFine = packageManager.CheckPermission("android.permission.ACCESS_FINE_LOCATION", packageName) == Permission.Granted;
                bool hasCoarse = packageManager.CheckPermission("android.permission.ACCESS_COARSE_LOCATION", packageName) == Permission.Granted;
                bool hasBg = packageManager.CheckPermission("android.permission.ACCESS_BACKGROUND_LOCATION", packageName) == Permission.Granted;

                for (int i = 0; i < packageInfo.RequestedPermissions.Count; i++)
                {
                    var permissionName = packageInfo.RequestedPermissions[i];
                    var category = Helpers.PermissionMapper.MapPermissionToCategory(permissionName);
                    
                    if (category == PermissionCategory.Unknown) continue;

                    bool isGranted = packageManager.CheckPermission(permissionName, packageName) == Permission.Granted;
                    
                    var status = PermissionAccessStatus.Denied;
                    if (isGranted)
                    {
                        status = PermissionAccessStatus.Allowed;

                        if (category == PermissionCategory.Location)
                        {
                            if (permissionName == "android.permission.ACCESS_FINE_LOCATION")
                            {
                                status = PermissionAccessStatus.Precise;
                            }
                            else if (permissionName == "android.permission.ACCESS_COARSE_LOCATION")
                            {
                                status = hasFine ? PermissionAccessStatus.Precise : PermissionAccessStatus.Approximate;
                            }
                            else if (permissionName == "android.permission.ACCESS_BACKGROUND_LOCATION")
                            {
                                status = PermissionAccessStatus.Allowed;
                            }
                        }
                    }

                    permissions.Add(new AppPermission
                    {
                        PermissionName = permissionName,
                        DisplayName = Helpers.PermissionMapper.GetFriendlyName(permissionName),
                        Category = category,
                        Status = status,
                        IsGranted = isGranted,
                        LastAccessTime = null,
                        UsageDataAvailability = DataAvailability.RestrictedByAndroid
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching permissions for {packageName}: {ex}");
            }
#endif
            return Task.FromResult<IEnumerable<AppPermission>>(permissions);
        }

        public Task<IEnumerable<DevicePermission>> GetSupportedPermissionsAsync(CancellationToken cancellationToken = default)
        {
            var permissions = new List<DevicePermission>();
#if ANDROID
            var sdkInt = (int)global::Android.OS.Build.VERSION.SdkInt;
#else
            var sdkInt = 33;
#endif

            var categories = new[]
            {
                (PermissionCategory.Camera, "Camera", "android.permission.CAMERA"),
                (PermissionCategory.Microphone, "Microphone", "android.permission.RECORD_AUDIO"),
                (PermissionCategory.Location, "Location", "android.permission.ACCESS_FINE_LOCATION"),
                (PermissionCategory.Contacts, "Contacts", "android.permission.READ_CONTACTS"),
                (PermissionCategory.PhotosAndVideos, "Photos & Videos", sdkInt >= 33 ? "android.permission.READ_MEDIA_IMAGES" : "android.permission.READ_EXTERNAL_STORAGE"),
                (PermissionCategory.MusicAndAudio, "Music & Audio", sdkInt >= 33 ? "android.permission.READ_MEDIA_AUDIO" : "android.permission.READ_EXTERNAL_STORAGE"),
                (PermissionCategory.Storage, "Storage", "android.permission.READ_EXTERNAL_STORAGE"),
                (PermissionCategory.Phone, "Phone", "android.permission.READ_PHONE_STATE"),
                (PermissionCategory.Sms, "SMS", "android.permission.READ_SMS"),
                (PermissionCategory.Calendar, "Calendar", "android.permission.READ_CALENDAR"),
                (PermissionCategory.NearbyDevices, "Nearby Devices", "android.permission.BLUETOOTH_SCAN"),
                (PermissionCategory.Notifications, "Notifications", "android.permission.POST_NOTIFICATIONS"),
                (PermissionCategory.PhysicalActivity, "Physical Activity", "android.permission.ACTIVITY_RECOGNITION"),
                (PermissionCategory.PhoneState, "Phone State", "android.permission.READ_PHONE_STATE"),
                (PermissionCategory.CallRecords, "Call Records", "android.permission.READ_CALL_LOG"),
                (PermissionCategory.DisplayOverOtherApps, "Display Over Other Apps", "android.permission.SYSTEM_ALERT_WINDOW"),
                (PermissionCategory.DiscoverContacts, "Discover Contacts", "android.permission.GET_ACCOUNTS"),
                (PermissionCategory.WriteCalendar, "Write Calendar Events", "android.permission.WRITE_CALENDAR"),
                (PermissionCategory.DeviceAdmin, "Device Admin", "android.permission.BIND_DEVICE_ADMIN"),
                (PermissionCategory.ScreenCapture, "Screen Capture / Recording", "android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION"),
                (PermissionCategory.Accessibility, "Accessibility", "android.permission.BIND_ACCESSIBILITY_SERVICE"),
                (PermissionCategory.NotificationAccess, "Notification Access", "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE"),
                (PermissionCategory.UsageDataAccess, "Usage Data Access", "android.permission.PACKAGE_USAGE_STATS"),
                (PermissionCategory.InstallUnknownApps, "Install Unknown Apps", "android.permission.REQUEST_INSTALL_PACKAGES"),
                (PermissionCategory.ManageAllFiles, "Manage All Files", "android.permission.MANAGE_EXTERNAL_STORAGE"),
                (PermissionCategory.VpnControl, "VPN Control", "android.permission.BIND_VPN_SERVICE"),
                (PermissionCategory.QueryAllPackages, "Query All Packages", "android.permission.QUERY_ALL_PACKAGES"),
                (PermissionCategory.ModifySystemSettings, "Modify System Settings", "android.permission.WRITE_SETTINGS")
            };

            foreach (var item in categories)
            {
                bool isSupported = true;
                
                if (item.Item1 == PermissionCategory.Notifications && sdkInt < 33)
                {
                    isSupported = false;
                }
                if (item.Item1 == PermissionCategory.PhotosAndVideos && sdkInt < 33 && item.Item3.Contains("READ_MEDIA_IMAGES"))
                {
                    isSupported = false;
                }
                if (item.Item1 == PermissionCategory.MusicAndAudio && sdkInt < 33 && item.Item3.Contains("READ_MEDIA_AUDIO"))
                {
                    isSupported = false;
                }

                permissions.Add(new DevicePermission
                {
                    PermissionName = item.Item3,
                    DisplayName = item.Item2,
                    Description = Helpers.PermissionMapper.GetCategoryDescription(item.Item1),
                    Category = item.Item1,
                    GrantedAppCount = 0,
                    IsSupported = isSupported
                });
            }

            return Task.FromResult<IEnumerable<DevicePermission>>(permissions);
        }

        public Task<IEnumerable<InstalledApp>> GetAppsWithPermissionAsync(PermissionCategory category, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<InstalledApp>>(Enumerable.Empty<InstalledApp>());
        }
    }
}
