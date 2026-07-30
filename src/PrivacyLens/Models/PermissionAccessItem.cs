using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class PermissionAccessItem
    {
        public string PackageName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string? IconCachePath { get; set; }
        public PermissionCategory Category { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public PermissionAccessStatus Status { get; set; }
        public bool IsGranted { get; set; }
        public DateTimeOffset? LastAccessTime { get; set; }
        public DataAvailability UsageDataAvailability { get; set; }
    }
}
