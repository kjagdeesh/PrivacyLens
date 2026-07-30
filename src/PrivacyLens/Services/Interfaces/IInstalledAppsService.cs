using PrivacyLens.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IInstalledAppsService
    {
        Task<IEnumerable<InstalledApp>> GetInstalledAppsAsync(CancellationToken cancellationToken = default);
        Task<InstalledApp?> GetAppAsync(string packageName, CancellationToken cancellationToken = default);
    }
}
