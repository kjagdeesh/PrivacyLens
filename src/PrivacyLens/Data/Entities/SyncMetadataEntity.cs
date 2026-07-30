using SQLite;
using System;
using PrivacyLens.Enums;

namespace PrivacyLens.Data.Entities
{
    [Table("SyncMetadata")]
    public class SyncMetadataEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public SyncType SyncType { get; set; }

        public DateTime? LastSuccessfulSyncAt { get; set; }
        public DateTime? LastAttemptedSyncAt { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
