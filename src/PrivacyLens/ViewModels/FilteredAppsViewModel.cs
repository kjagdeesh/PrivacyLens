using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrivacyLens.Models;
using PrivacyLens.Repositories.Interfaces;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyLens.ViewModels
{
    [QueryProperty(nameof(FilterType), "filterType")]
    public partial class FilteredAppsViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private readonly IRefreshService _refreshService;

        private string _filterType = string.Empty;
        public string FilterType
        {
            get => _filterType;
            set
            {
                if (SetProperty(ref _filterType, value))
                {
                    UpdateTextBasedOnFilter();
                    Task.Run(async () => await LoadAppsAsync());
                }
            }
        }

        [ObservableProperty]
        private string _pageTitle = string.Empty;

        [ObservableProperty]
        private string _pageDescription = string.Empty;

        [ObservableProperty]
        private string _infoEmoji = string.Empty;

        [ObservableProperty]
        private bool _isSensitive;

        [ObservableProperty]
        private bool _isHighRisk;

        [ObservableProperty]
        private ObservableCollection<InstalledApp> _apps = new();

        public FilteredAppsViewModel(IPrivacyRepository repository, IRefreshService refreshService)
        {
            _repository = repository;
            _refreshService = refreshService;
            
            _refreshService.SyncStatusChanged += OnSyncStatusChanged;
        }

        private void OnSyncStatusChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => IsRefreshing = _refreshService.IsSyncing);
            Task.Run(async () => await LoadAppsAsync(false));
        }

        private void UpdateTextBasedOnFilter()
        {
            IsSensitive = string.Equals(FilterType, "Sensitive", StringComparison.OrdinalIgnoreCase);
            IsHighRisk = string.Equals(FilterType, "HighRisk", StringComparison.OrdinalIgnoreCase);

            if (IsSensitive)
            {
                PageTitle = "Apps with Sensitive Access";
                InfoEmoji = "ic_lock_open.png";
                PageDescription = "Apps that have been granted at least one sensitive permission — such as Camera, Microphone, Location, Contacts, or Storage.\n\nA granted permission doesn't necessarily mean the app is actively using it, but it does mean the app has the capability to access that resource at any time.";
            }
            else if (IsHighRisk)
            {
                PageTitle = "High-Risk Apps";
                InfoEmoji = "ic_warning.png";
                PageDescription = "Apps that have been granted 3 or more sensitive permissions simultaneously.\n\nThese apps have broad access to your personal data. We highly recommend reviewing these apps to ensure they actually need all the permissions they've been granted.";
            }
        }

        [RelayCommand]
        public async Task LoadAppsAsync(bool checkFreshness = true)
        {
            if (IsBusy) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                var filtered = await Task.Run(async () =>
                {
                    if (checkFreshness)
                        await _refreshService.EnsureFreshDataAsync();

                    var allApps = await _repository.GetAppsAsync();
                    
                    if (string.Equals(FilterType, "Sensitive", StringComparison.OrdinalIgnoreCase))
                    {
                        return allApps.Where(a => a.GrantedSensitivePermissionCount > 0).ToList();
                    }
                    else if (string.Equals(FilterType, "HighRisk", StringComparison.OrdinalIgnoreCase))
                    {
                        return allApps.Where(a => a.GrantedSensitivePermissionCount >= 3).ToList();
                    }
                    return new List<InstalledApp>();
                });

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Apps = new ObservableCollection<InstalledApp>(filtered);
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HasError = true;
                    ErrorMessage = "Failed to load filtered apps list.";
                });
                System.Diagnostics.Debug.WriteLine($"[FilteredAppsViewModel] LoadApps error: {ex}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                    IsRefreshing = false;
                });
            }
        }

        [RelayCommand]
        public async Task NavigateToAppDetailsAsync(InstalledApp app)
        {
            if (app == null) return;
            await Shell.Current.GoToAsync($"appdetails?packageName={app.PackageName}");
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadAppsAsync();
        }
    }
}
