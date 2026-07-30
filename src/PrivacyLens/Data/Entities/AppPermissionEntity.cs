using SQLite;
using System;
using PrivacyLens.Enums;

namespace PrivacyLens.Data.Entities
{
    [Table("AppPermissions")]
    public class AppPermissionEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Name = "UX_PackageName_PermissionName", Unique = true, Order = 1)]
        public string PackageName { get; set; } = string.Empty;

        [Indexed(Name = "UX_PackageName_PermissionName", Unique = true, Order = 2)]
        public string PermissionName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        [Indexed]
        public PermissionCategory PermissionCategory { get; set; }

        public PermissionAccessStatus PermissionAccessStatus { get; set; }

        [Indexed]
        public bool IsGranted { get; set; }

        public DateTime? LastAccessTime { get; set; }

        public DataAvailability UsageDataAvailability { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}
