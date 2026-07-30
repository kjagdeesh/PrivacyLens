using PrivacyLens.Enums;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PrivacyLens.Converters
{
    /// <summary>
    /// Converts a <see cref="PermissionCategory"/> to a Material Design icon image filename.
    /// The returned filename must exist in Resources/Images/ as a PNG or SVG file.
    /// </summary>
    public class PermissionIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PermissionCategory category)
            {
                return category switch
                {
                    PermissionCategory.Camera          => "ic_camera.png",
                    PermissionCategory.Microphone      => "ic_mic.png",
                    PermissionCategory.Location        => "ic_location_on.png",
                    PermissionCategory.Contacts        => "ic_contacts.png",
                    PermissionCategory.PhotosAndVideos => "ic_photo.png",
                    PermissionCategory.MusicAndAudio   => "ic_music_note.png",
                    PermissionCategory.Storage         => "ic_folder.png",
                    PermissionCategory.Phone           => "ic_phone.png",
                    PermissionCategory.Sms             => "ic_sms.png",
                    PermissionCategory.Calendar        => "ic_event.png",
                    PermissionCategory.NearbyDevices   => "ic_bluetooth.png",
                    PermissionCategory.Notifications   => "ic_notifications.png",
                    PermissionCategory.PhysicalActivity=> "ic_directions_run.png",
                    PermissionCategory.PhoneState      => "ic_phone_android.png",
                    PermissionCategory.CallRecords     => "ic_call_log.png",
                    PermissionCategory.DisplayOverOtherApps => "ic_layers.png",
                    PermissionCategory.DiscoverContacts=> "ic_manage_accounts.png",
                    PermissionCategory.WriteCalendar   => "ic_edit_calendar.png",
                    PermissionCategory.DeviceAdmin     => "ic_admin_panel_settings.png",
                    PermissionCategory.ScreenCapture   => "ic_screen_record.png",
                    PermissionCategory.Accessibility   => "ic_accessibility.png",
                    PermissionCategory.NotificationAccess => "ic_mark_email_read.png",
                    PermissionCategory.UsageDataAccess => "ic_data_usage.png",
                    PermissionCategory.InstallUnknownApps => "ic_install_mobile.png",
                    PermissionCategory.ManageAllFiles  => "ic_manage_history.png",
                    PermissionCategory.VpnControl      => "ic_vpn_key.png",
                    PermissionCategory.QueryAllPackages=> "ic_apps.png",
                    PermissionCategory.ModifySystemSettings => "ic_settings.png",
                    _                                  => "ic_shield.png",
                };
            }
            return "ic_shield.png";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
