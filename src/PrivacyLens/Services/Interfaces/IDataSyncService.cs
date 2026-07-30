using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IDataSyncService
    {
        Task<bool> IsRefreshRequiredAsync(CancellationToken cancellationToken = default);
        Task SyncAsync(bool forceRefresh, CancellationToken cancellationToken = default);
    }
}
