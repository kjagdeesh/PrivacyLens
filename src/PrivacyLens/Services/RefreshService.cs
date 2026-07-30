using PrivacyLens.Repositories.Interfaces;
using PrivacyLens.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services
{
    public class RefreshService : IRefreshService
    {
        private readonly IDataSyncService _dataSyncService;
        private readonly IPrivacyRepository _privacyRepository;
        
        private bool _isSyncing;
        private DateTimeOffset? _lastSuccessfulSyncAt;

        public bool IsSyncing
        {
            get => _isSyncing;
            private set
            {
                if (_isSyncing != value)
                {
                    _isSyncing = value;
                    SyncStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public DateTimeOffset? LastSuccessfulSyncAt
        {
            get => _lastSuccessfulSyncAt;
            private set
            {
                if (_lastSuccessfulSyncAt != value)
                {
                    _lastSuccessfulSyncAt = value;
                    SyncStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? SyncStatusChanged;

        public RefreshService(IDataSyncService dataSyncService, IPrivacyRepository privacyRepository)
        {
            _dataSyncService = dataSyncService;
            _privacyRepository = privacyRepository;
            
            _ = LoadLastSyncTimeAsync();
        }

        private async Task LoadLastSyncTimeAsync()
        {
            try
            {
                LastSuccessfulSyncAt = await _privacyRepository.GetLastSuccessfulSyncTimeAsync();
            }
            catch
            {
                // Suppress errors during constructor load
            }
        }

        public async Task EnsureFreshDataAsync(CancellationToken cancellationToken = default)
        {
            var isStale = await _dataSyncService.IsRefreshRequiredAsync(cancellationToken);
            if (isStale)
            {
                await ForceRefreshAsync(cancellationToken);
            }
        }

        public async Task ForceRefreshAsync(CancellationToken cancellationToken = default)
        {
            if (IsSyncing) return;

            IsSyncing = true;
            try
            {
                await Task.Run(() => _dataSyncService.SyncAsync(forceRefresh: true, cancellationToken), cancellationToken);
                LastSuccessfulSyncAt = await _privacyRepository.GetLastSuccessfulSyncTimeAsync();
            }
            finally
            {
                IsSyncing = false;
            }
        }
    }
}
