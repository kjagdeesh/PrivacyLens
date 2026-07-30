using SQLite;
using PrivacyLens.Data.Entities;
using System.Threading.Tasks;

namespace PrivacyLens.Data
{
    public class PrivacyLensDatabase
    {
        private SQLiteAsyncConnection? _database;
        private readonly System.Threading.SemaphoreSlim _initSemaphore = new(1, 1);
        private bool _isInitialized;

        public PrivacyLensDatabase()
        {
        }

        public async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (_isInitialized && _database != null)
            {
                return _database;
            }

            await _initSemaphore.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    _database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
                    
                    // Create tables asynchronously
                    await _database.CreateTableAsync<InstalledAppEntity>();
                    await _database.CreateTableAsync<AppPermissionEntity>();
                    await _database.CreateTableAsync<DevicePermissionEntity>();
                    await _database.CreateTableAsync<PermissionUsageEntity>();
                    await _database.CreateTableAsync<PermissionCapabilityEntity>();
                    await _database.CreateTableAsync<SyncMetadataEntity>();

                    _isInitialized = true;
                }
            }
            finally
            {
                _initSemaphore.Release();
            }

            return _database!;
        }
    }
}
