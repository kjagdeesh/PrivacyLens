using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Services.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#if ANDROID
using Android.App;
using Android.Content;
#endif

namespace PrivacyLens.Platforms.Android.Services
{
    public class AndroidDeviceCapabilityService : IDeviceCapabilityService
    {
        public Task<IEnumerable<PermissionCapability>> GetPermissionCapabilitiesAsync(CancellationToken cancellationToken = default)
        {
            var capabilities = new List<PermissionCapability>();
#if ANDROID
            var sdkInt = (int)global::Android.OS.Build.VERSION.SdkInt;
#else
            var sdkInt = 33;
#endif

            var categories = new[]
            {
                PermissionCategory.Camera,
                PermissionCategory.Microphone,
                PermissionCategory.Location,
                PermissionCategory.Contacts,
                PermissionCategory.PhotosAndVideos,
                PermissionCategory.MusicAndAudio,
                PermissionCategory.Storage,
                PermissionCategory.Phone,
                PermissionCategory.Sms,
                PermissionCategory.Calendar,
                PermissionCategory.NearbyDevices,
                PermissionCategory.Notifications,
                PermissionCategory.PhysicalActivity,
                PermissionCategory.PhoneState,
                PermissionCategory.CallRecords,
                PermissionCategory.DisplayOverOtherApps,
                PermissionCategory.DiscoverContacts,
                PermissionCategory.WriteCalendar,
                PermissionCategory.DeviceAdmin,
                PermissionCategory.ScreenCapture,
                PermissionCategory.Accessibility,
                PermissionCategory.NotificationAccess,
                PermissionCategory.UsageDataAccess,
                PermissionCategory.InstallUnknownApps,
                PermissionCategory.ManageAllFiles,
                PermissionCategory.VpnControl,
                PermissionCategory.QueryAllPackages,
                PermissionCategory.ModifySystemSettings
            };

            foreach (var category in categories)
            {
                bool isSupported = true;
                bool canReadGranted = true;
                bool canReadLastUsage = true;
                string? limitations = null;

                if (category == PermissionCategory.Notifications && sdkInt < 33)
                {
                    isSupported = false;
                    limitations = "Runtime notification permission is not supported on Android versions below 13 (API 33).";
                }
                else if (category == PermissionCategory.PhotosAndVideos && sdkInt < 33)
                {
                    limitations = "Photos and videos are grouped under legacy Storage permissions on versions below 13.";
                }
                else if (category == PermissionCategory.MusicAndAudio && sdkInt < 33)
                {
                    limitations = "Audio files are grouped under legacy Storage permissions on versions below 13.";
                }

                if (isSupported)
                {
                    limitations = limitations ?? "Last access time requires system/platform privileges. Regular third-party apps cannot query historical access times.";
                }

                capabilities.Add(new PermissionCapability
                {
                    Category = category,
                    IsSupported = isSupported,
                    CanReadGrantedStatus = canReadGranted,
                    CanReadLastUsage = canReadLastUsage,
                    RequiresSpecialAccess = false,
                    LimitationDescription = limitations
                });
            }

            return Task.FromResult<IEnumerable<PermissionCapability>>(capabilities);
        }

        public bool IsUsageAccessRequired()
        {
            // We set it to false since we do not require Usage Stats permissions for core v1 dashboard features.
            return false;
        }

        public bool HasUsageAccess()
        {
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var appOps = (global::Android.App.AppOpsManager?)context.GetSystemService(global::Android.Content.Context.AppOpsService);
                if (appOps == null) return false;

                var mode = appOps.NoteOpNoThrow(
                    global::Android.App.AppOpsManager.OpstrGetUsageStats,
                    global::Android.OS.Process.MyUid(),
                    context.PackageName);
                
                return mode == global::Android.App.AppOpsManagerMode.Allowed;
            }
            catch
            {
                return false;
            }
#else
            return true;
#endif
        }
    }
}
