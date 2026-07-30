namespace PrivacyLens.Models
{
    public class InstalledApp
    {
        public string PackageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? VersionName { get; set; }
        public long? VersionCode { get; set; }
        public bool IsSystemApp { get; set; }
        public string? IconCachePath { get; set; }
        public int GrantedSensitivePermissionCount { get; set; }
        public string? AppProvidedDescription { get; set; }

        public string Description => IsSystemApp 
            ? "Core system component or built-in app." 
            : "Third-party application installed by user.";
    }
}
