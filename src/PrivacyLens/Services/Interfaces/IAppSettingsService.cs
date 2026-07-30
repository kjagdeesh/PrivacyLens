using System.Threading.Tasks;

namespace PrivacyLens.Services.Interfaces
{
    public interface IAppSettingsService
    {
        Task OpenApplicationSettingsAsync(string packageName);
        Task OpenUsageAccessSettingsAsync();
        Task OpenPrivacySettingsAsync();
    }
}
