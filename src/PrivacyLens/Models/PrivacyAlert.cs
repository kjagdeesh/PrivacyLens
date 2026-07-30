using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class PrivacyAlert
    {
        public Guid Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public PermissionCategory PermissionCategory { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public PrivacyAlertSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset DetectedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
