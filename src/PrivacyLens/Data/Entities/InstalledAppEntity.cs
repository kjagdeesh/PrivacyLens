using SQLite;
using System;

namespace PrivacyLens.Data.Entities
{
    [Table("InstalledApps")]
    public class InstalledAppEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string PackageName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string? VersionName { get; set; }
        public long? VersionCode { get; set; }
        public bool IsSystemApp { get; set; }
        public string? IconCachePath { get; set; }
        public string? AppProvidedDescription { get; set; }
        public int GrantedSensitivePermissionCount { get; set; }
        public DateTime FirstDetectedAt { get; set; }
        public DateTime LastDetectedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
