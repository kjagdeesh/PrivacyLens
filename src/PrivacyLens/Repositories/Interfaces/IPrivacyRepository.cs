using PrivacyLens.Enums;
using PrivacyLens.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivacyLens.Repositories.Interfaces
{
    public interface IPrivacyRepository
    {
        Task<IEnumerable<InstalledApp>> GetAppsAsync();
        Task<InstalledApp?> GetAppAsync(string packageName);
        Task<IEnumerable<AppPermission>> GetAppPermissionsAsync(string packageName);
        Task<IEnumerable<DevicePermission>> GetPermissionsAsync();
        Task<IEnumerable<InstalledApp>> GetAppsForPermissionAsync(PermissionCategory category);
        Task<List<PermissionAccessItem>> GetPermissionAccessItemsAsync(PermissionCategory category);
        Task<IEnumerable<PermissionUsageRecord>> GetTodayPermissionActivityAsync();
        Task<DateTimeOffset?> GetLastSuccessfulSyncTimeAsync();
    }
}
