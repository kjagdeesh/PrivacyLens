using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class PermissionCapability
    {
        public PermissionCategory Category { get; set; }
        public bool IsSupported { get; set; }
        public bool CanReadGrantedStatus { get; set; }
        public bool CanReadLastUsage { get; set; }
        public bool RequiresSpecialAccess { get; set; }
        public string? LimitationDescription { get; set; }
    }
}
