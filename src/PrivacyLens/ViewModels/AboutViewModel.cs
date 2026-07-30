using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace PrivacyLens.ViewModels
{
    public partial class AboutViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _appVersion;

        public AboutViewModel()
        {
            _appVersion = $"VERSION {AppInfo.Current.VersionString}";
        }

        [RelayCommand]
        private async Task OpenPrivacyPolicyAsync()
        {
            //await Browser.Default.OpenAsync("https://raw.githubusercontent.com/kjagdeesh/PrivacyLens/refs/heads/main/PRIVACY_POLICY.md", BrowserLaunchMode.SystemPreferred);
            await Browser.Default.OpenAsync("https://github.com/kjagdeesh/PrivacyLens/blob/main/PRIVACY_POLICY.md", BrowserLaunchMode.SystemPreferred);
        }

        [RelayCommand]
        private async Task OpenTermsAsync()
        {
            //await Browser.Default.OpenAsync("https://raw.githubusercontent.com/kjagdeesh/PrivacyLens/refs/heads/main/TERMS_OF_USE.md", BrowserLaunchMode.SystemPreferred);
            await Browser.Default.OpenAsync("https://github.com/kjagdeesh/PrivacyLens/blob/main/TERMS_OF_USE.md", BrowserLaunchMode.SystemPreferred);
        }
        [RelayCommand]
        private async Task OpenGithubAsync()
        {
            await Browser.Default.OpenAsync("https://github.com/kjagdeesh/PrivacyLens", BrowserLaunchMode.SystemPreferred);
        }
    }
}
