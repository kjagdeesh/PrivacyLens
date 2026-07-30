using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class AppPermission
    {
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public PermissionCategory Category { get; set; }
        public PermissionAccessStatus Status { get; set; }
        public bool IsGranted { get; set; }
        public DateTimeOffset? LastAccessTime { get; set; }
        public DataAvailability UsageDataAvailability { get; set; }
    }
}
