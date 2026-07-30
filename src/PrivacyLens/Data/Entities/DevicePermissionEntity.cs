using SQLite;
using System;
using PrivacyLens.Enums;

namespace PrivacyLens.Data.Entities
{
    [Table("DevicePermissions")]
    public class DevicePermissionEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string PermissionName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Indexed]
        public PermissionCategory PermissionCategory { get; set; }

        public int GrantedAppCount { get; set; }
        public bool IsSupported { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
