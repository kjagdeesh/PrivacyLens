using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class PermissionUsageRecord
    {
        public string PackageName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public PermissionCategory Category { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public DateTimeOffset? AccessTime { get; set; }
        public bool IsBackgroundAccess { get; set; }
        public bool IsCurrentlyActive { get; set; }
        public DataAvailability DataAvailability { get; set; }
        public string IconCachePath { get; set; } = string.Empty;
    }
}
