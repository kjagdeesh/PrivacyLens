using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class DevicePermission
    {
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PermissionCategory Category { get; set; }
        public int GrantedAppCount { get; set; }
        public bool IsSupported { get; set; }
    }
}
