using PrivacyLens.Enums;
using PrivacyLens.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IPermissionUsageService
    {
        Task<IEnumerable<PermissionUsageRecord>> GetTodayUsageAsync(CancellationToken cancellationToken = default);
        Task<PermissionUsageRecord?> GetLastUsageAsync(string packageName, PermissionCategory category, CancellationToken cancellationToken = default);
        bool HasUsageAccess();
        void RequestUsageAccess();
    }
}
