using SQLite;
using System;
using PrivacyLens.Enums;

namespace PrivacyLens.Data.Entities
{
    [Table("PermissionUsages")]
    public class PermissionUsageEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string PackageName { get; set; } = string.Empty;

        public string AppName { get; set; } = string.Empty;

        [Indexed]
        public PermissionCategory PermissionCategory { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        [Indexed]
        public DateTime? AccessTime { get; set; }

        public bool IsBackgroundAccess { get; set; }
        public bool IsCurrentlyActive { get; set; }
        public DataAvailability DataAvailability { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
