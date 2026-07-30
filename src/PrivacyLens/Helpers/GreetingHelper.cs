using System;

namespace PrivacyLens.Helpers
{
    public static class GreetingHelper
    {
        public static string GetGreeting(string? username = null)
        {
            return GetGreeting(DateTime.Now, username);
        }

        public static string GetGreeting(DateTime time, string? username = null)
        {
            var hour = time.Hour;
            string timeOfDay = hour switch
            {
                < 12 => "Morning",
                < 17 => "Afternoon",
                _ => "Evening"
            };

            return string.IsNullOrEmpty(username)
                ? $"Good {timeOfDay}"
                : $"Good {timeOfDay}, {username}";
        }
    }
}
