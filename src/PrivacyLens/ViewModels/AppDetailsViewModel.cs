using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrivacyLens.Models;
using PrivacyLens.Repositories.Interfaces;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PrivacyLens.ViewModels
{
    [QueryProperty(nameof(PackageName), "packageName")]
    public partial class AppDetailsViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private readonly IAppSettingsService _settingsService;
        
        private string _packageName = string.Empty;

        [ObservableProperty]
        private InstalledApp? _app;

        [ObservableProperty]
        private ObservableCollection<AppPermission> _permissions = new();

        public string PackageName
        {
            get => _packageName;
            set
            {
                _packageName = value;
                _ = LoadAppDetailsAsync();
            }
        }

        public AppDetailsViewModel(IPrivacyRepository repository, IAppSettingsService settingsService)
        {
            _repository = repository;
            _settingsService = settingsService;
        }

        public async Task LoadAppDetailsAsync()
        {
            if (string.IsNullOrEmpty(PackageName)) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                var (app, perms) = await Task.Run(async () =>
                {
                    var a = await _repository.GetAppAsync(PackageName);
                    var p = await _repository.GetAppPermissionsAsync(PackageName);
                    return (a, p);
                });

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    App = app;
                    Permissions = new ObservableCollection<AppPermission>(perms);
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HasError = true;
                    ErrorMessage = "Failed to load application details.";
                });
                System.Diagnostics.Debug.WriteLine($"[AppDetailsViewModel] Error: {ex}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
            }
        }

        [RelayCommand]
        public async Task ManagePermissionsAsync()
        {
            if (App == null) return;
            await _settingsService.OpenApplicationSettingsAsync(App.PackageName);
        }
    }
}
