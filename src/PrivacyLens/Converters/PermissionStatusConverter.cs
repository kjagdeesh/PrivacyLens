using PrivacyLens.Enums;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PrivacyLens.Converters
{
    public class PermissionStatusConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PermissionAccessStatus status)
            {
                return status switch
                {
                    PermissionAccessStatus.Allowed => "Allowed",
                    PermissionAccessStatus.Denied => "Denied",
                    PermissionAccessStatus.AllowedWhileUsingApp => "While using app",
                    PermissionAccessStatus.AskEveryTime => "Ask every time",
                    PermissionAccessStatus.Approximate => "Approximate",
                    PermissionAccessStatus.Precise => "Precise",
                    PermissionAccessStatus.Restricted => "Restricted",
                    _ => "Unknown"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
