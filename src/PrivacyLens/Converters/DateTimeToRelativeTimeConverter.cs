using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PrivacyLens.Converters
{
    public class DateTimeToRelativeTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return "Not recently opened";

            DateTime dateTime;
            if (value is DateTimeOffset dto)
            {
                dateTime = dto.UtcDateTime;
            }
            else if (value is DateTime dt)
            {
                dateTime = dt;
            }
            else
            {
                return "Not recently opened";
            }

            var difference = DateTime.UtcNow - dateTime;
            string timeString;

            if (difference.TotalSeconds < 60)
                timeString = "Just now";
            else if (difference.TotalMinutes < 60)
                timeString = $"{(int)difference.TotalMinutes} min ago";
            else if (difference.TotalHours < 24)
                timeString = $"{(int)difference.TotalHours} hours ago";
            else if (difference.TotalDays < 2)
                timeString = "Yesterday";
            else if (difference.TotalDays < 30)
                timeString = $"{(int)difference.TotalDays} days ago";
            else
                timeString = dateTime.ToLocalTime().ToString("g");

            return $"Opened: {timeString}";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
