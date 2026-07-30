using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IRefreshService
    {
        bool IsSyncing { get; }
        DateTimeOffset? LastSuccessfulSyncAt { get; }
        
        event EventHandler? SyncStatusChanged;
        
        Task EnsureFreshDataAsync(CancellationToken cancellationToken = default);
        Task ForceRefreshAsync(CancellationToken cancellationToken = default);
    }
}
