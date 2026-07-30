using SQLite;
using System;
using PrivacyLens.Enums;

namespace PrivacyLens.Data.Entities
{
    [Table("PermissionCapabilities")]
    public class PermissionCapabilityEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public PermissionCategory PermissionCategory { get; set; }

        public bool IsSupported { get; set; }
        public bool CanReadGrantedStatus { get; set; }
        public bool CanReadLastUsage { get; set; }
        public bool RequiresSpecialAccess { get; set; }
        public string? LimitationDescription { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
