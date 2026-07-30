using PrivacyLens.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IDeviceCapabilityService
    {
        Task<IEnumerable<PermissionCapability>> GetPermissionCapabilitiesAsync(CancellationToken cancellationToken = default);
        bool IsUsageAccessRequired();
        bool HasUsageAccess();
    }
}
