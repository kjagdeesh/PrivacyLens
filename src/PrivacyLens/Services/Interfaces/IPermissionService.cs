using PrivacyLens.Enums;
using PrivacyLens.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<AppPermission>> GetPermissionsForAppAsync(string packageName, CancellationToken cancellationToken = default);
        Task<IEnumerable<DevicePermission>> GetSupportedPermissionsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<InstalledApp>> GetAppsWithPermissionAsync(PermissionCategory category, CancellationToken cancellationToken = default);
    }
}
