using PrivacyLens.Enums;

namespace PrivacyLens.Models
{
    public class SyncResult
    {
        public SyncType Type { get; set; }
        public DateTimeOffset AttemptedAt { get; set; }
        public SyncStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public int RecordsSynced { get; set; }
    }
}
