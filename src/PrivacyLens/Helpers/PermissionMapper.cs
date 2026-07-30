using PrivacyLens.Enums;
using System.Collections.Generic;

namespace PrivacyLens.Helpers
{
    public static class PermissionMapper
    {
        private static readonly Dictionary<string, PermissionCategory> PermissionMap = new()
        {
            { "android.permission.CAMERA", PermissionCategory.Camera },
            
            { "android.permission.RECORD_AUDIO", PermissionCategory.Microphone },
            
            { "android.permission.ACCESS_FINE_LOCATION", PermissionCategory.Location },
            { "android.permission.ACCESS_COARSE_LOCATION", PermissionCategory.Location },
            { "android.permission.ACCESS_BACKGROUND_LOCATION", PermissionCategory.Location },
            
            { "android.permission.READ_CONTACTS", PermissionCategory.Contacts },
            { "android.permission.WRITE_CONTACTS", PermissionCategory.Contacts },
            { "android.permission.GET_ACCOUNTS", PermissionCategory.DiscoverContacts },
            
            { "android.permission.READ_SMS", PermissionCategory.Sms },
            { "android.permission.SEND_SMS", PermissionCategory.Sms },
            { "android.permission.RECEIVE_SMS", PermissionCategory.Sms },
            { "android.permission.RECEIVE_MMS", PermissionCategory.Sms },
            { "android.permission.RECEIVE_WAP_PUSH", PermissionCategory.Sms },
            
            { "android.permission.READ_PHONE_STATE", PermissionCategory.PhoneState },
            { "android.permission.CALL_PHONE", PermissionCategory.Phone },
            { "android.permission.READ_CALL_LOG", PermissionCategory.CallRecords },
            { "android.permission.WRITE_CALL_LOG", PermissionCategory.CallRecords },
            { "android.permission.ADD_VOICEMAIL", PermissionCategory.Phone },
            { "android.permission.USE_SIP", PermissionCategory.Phone },
            
            { "android.permission.READ_CALENDAR", PermissionCategory.Calendar },
            { "android.permission.WRITE_CALENDAR", PermissionCategory.WriteCalendar },
            
            { "android.permission.READ_MEDIA_IMAGES", PermissionCategory.PhotosAndVideos },
            { "android.permission.READ_MEDIA_VIDEO", PermissionCategory.PhotosAndVideos },
            
            { "android.permission.READ_MEDIA_AUDIO", PermissionCategory.MusicAndAudio },
            
            { "android.permission.READ_EXTERNAL_STORAGE", PermissionCategory.Storage },
            { "android.permission.WRITE_EXTERNAL_STORAGE", PermissionCategory.Storage },
            
            { "android.permission.POST_NOTIFICATIONS", PermissionCategory.Notifications },
            
            { "android.permission.BLUETOOTH_SCAN", PermissionCategory.NearbyDevices },
            { "android.permission.BLUETOOTH_CONNECT", PermissionCategory.NearbyDevices },
            { "android.permission.BLUETOOTH_ADVERTISE", PermissionCategory.NearbyDevices },
            
            { "android.permission.ACTIVITY_RECOGNITION", PermissionCategory.PhysicalActivity },
            
            { "android.permission.SYSTEM_ALERT_WINDOW", PermissionCategory.DisplayOverOtherApps },
            
            { "android.permission.BIND_DEVICE_ADMIN", PermissionCategory.DeviceAdmin },
            
            { "android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION", PermissionCategory.ScreenCapture },
            
            { "android.permission.BIND_ACCESSIBILITY_SERVICE", PermissionCategory.Accessibility },
            { "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE", PermissionCategory.NotificationAccess },
            { "android.permission.PACKAGE_USAGE_STATS", PermissionCategory.UsageDataAccess },
            { "android.permission.REQUEST_INSTALL_PACKAGES", PermissionCategory.InstallUnknownApps },
            { "android.permission.MANAGE_EXTERNAL_STORAGE", PermissionCategory.ManageAllFiles },
            { "android.permission.BIND_VPN_SERVICE", PermissionCategory.VpnControl },
            { "android.permission.QUERY_ALL_PACKAGES", PermissionCategory.QueryAllPackages },
            { "android.permission.WRITE_SETTINGS", PermissionCategory.ModifySystemSettings }
        };

        public static PermissionCategory MapPermissionToCategory(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName)) return PermissionCategory.Unknown;
            
            if (PermissionMap.TryGetValue(permissionName, out var category))
            {
                return category;
            }
            return PermissionCategory.Unknown;
        }

        public static string GetFriendlyName(string permissionName)
        {
            return permissionName switch
            {
                "android.permission.CAMERA" => "Camera",
                "android.permission.RECORD_AUDIO" => "Microphone",
                "android.permission.ACCESS_FINE_LOCATION" => "Precise Location",
                "android.permission.ACCESS_COARSE_LOCATION" => "Approximate Location",
                "android.permission.ACCESS_BACKGROUND_LOCATION" => "Background Location",
                "android.permission.READ_CONTACTS" => "Read Contacts",
                "android.permission.WRITE_CONTACTS" => "Write Contacts",
                "android.permission.GET_ACCOUNTS" => "Discover Accounts",
                "android.permission.READ_SMS" => "Read SMS Messages",
                "android.permission.SEND_SMS" => "Send SMS Messages",
                "android.permission.RECEIVE_SMS" => "Receive SMS Messages",
                "android.permission.READ_PHONE_STATE" => "Read Phone Status",
                "android.permission.CALL_PHONE" => "Direct Phone Dialing",
                "android.permission.READ_CALENDAR" => "Read Calendar Events",
                "android.permission.WRITE_CALENDAR" => "Write Calendar Events",
                "android.permission.READ_MEDIA_IMAGES" => "Photos Access",
                "android.permission.READ_MEDIA_VIDEO" => "Videos Access",
                "android.permission.READ_MEDIA_AUDIO" => "Audio Access",
                "android.permission.READ_EXTERNAL_STORAGE" => "Read Storage Files",
                "android.permission.WRITE_EXTERNAL_STORAGE" => "Modify Storage Files",
                "android.permission.POST_NOTIFICATIONS" => "Show Notifications",
                "android.permission.BLUETOOTH_SCAN" => "Nearby Device Bluetooth Scan",
                "android.permission.BLUETOOTH_CONNECT" => "Nearby Device Bluetooth Connect",
                "android.permission.ACTIVITY_RECOGNITION" => "Physical Activity Tracking",
                _ => permissionName.Replace("android.permission.", "")
            };
        }

        public static string GetCategoryDescription(PermissionCategory category)
        {
            return category switch
            {
                PermissionCategory.Camera => "Allows apps to take pictures and record videos.",
                PermissionCategory.Microphone => "Allows apps to record audio and capture speech input.",
                PermissionCategory.Location => "Allows apps to access your geographical location.",
                PermissionCategory.Contacts => "Allows apps to read and manage contact list info.",
                PermissionCategory.PhotosAndVideos => "Allows apps to access photos and videos on your device.",
                PermissionCategory.MusicAndAudio => "Allows apps to access audio and music files on your device.",
                PermissionCategory.Storage => "Allows apps to access photos, media, and files on legacy storage.",
                PermissionCategory.Phone => "Allows apps to make phone calls and query telephony status.",
                PermissionCategory.Sms => "Allows apps to send, read, and process text messages.",
                PermissionCategory.Calendar => "Allows apps to read, edit, or create calendar schedule events.",
                PermissionCategory.NearbyDevices => "Allows apps to discover, connect to, and pair with nearby devices.",
                PermissionCategory.Notifications => "Allows apps to post and show alerts in the notification bar.",
                PermissionCategory.PhysicalActivity => "Allows apps to track step count and detect physical motion.",
                PermissionCategory.PhoneState => "Allows apps to read phone status and identity.",
                PermissionCategory.CallRecords => "Allows apps to read and write your phone call logs.",
                PermissionCategory.DisplayOverOtherApps => "Allows apps to draw over other applications.",
                PermissionCategory.DiscoverContacts => "Allows apps to discover accounts added to your device.",
                PermissionCategory.WriteCalendar => "Allows apps to add or modify calendar events without your knowledge.",
                PermissionCategory.DeviceAdmin => "Allows apps to act as device administrators with elevated control.",
                PermissionCategory.ScreenCapture => "Allows apps to capture or record the contents of your screen.",
                PermissionCategory.Accessibility => "Allows apps to read the screen, log keystrokes, and interact with the UI.",
                PermissionCategory.NotificationAccess => "Allows apps to read incoming notifications, including OTPs and messages.",
                PermissionCategory.UsageDataAccess => "Allows apps to track which applications you open and for how long.",
                PermissionCategory.InstallUnknownApps => "Allows apps to download and prompt the installation of other applications.",
                PermissionCategory.ManageAllFiles => "Allows apps complete, unrestricted access to the entire file system.",
                PermissionCategory.VpnControl => "Allows apps to route and potentially intercept all device network traffic.",
                PermissionCategory.QueryAllPackages => "Allows apps to see every application installed on the device.",
                PermissionCategory.ModifySystemSettings => "Allows apps to alter core system settings like WiFi and brightness.",
                _ => "General system permissions."
            };
        }
    }
}
