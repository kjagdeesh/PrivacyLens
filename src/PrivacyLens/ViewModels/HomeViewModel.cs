using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrivacyLens.Models;
using PrivacyLens.Repositories.Interfaces;
using PrivacyLens.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyLens.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private readonly IRefreshService _refreshService;
        private readonly IPermissionUsageService _usageService;

        [ObservableProperty]
        private string _greeting = string.Empty;

        [ObservableProperty]
        private string _lastUpdatedText = "Never updated";

        [ObservableProperty]
        private bool _isActivityEmpty = true;
        
        [ObservableProperty]
        private bool _hasUsageAccess;

        /// <summary>Total number of apps tracked in the local database.</summary>
        [ObservableProperty]
        private int _totalAppsTracked;

        /// <summary>Number of apps that have at least one sensitive permission granted.</summary>
        [ObservableProperty]
        private int _appsWithSensitiveAccess;

        /// <summary>Number of apps with 3 or more sensitive permissions granted.</summary>
        [ObservableProperty]
        private int _highRiskAppsCount;

        /// <summary>True once stats have been loaded from the DB.</summary>
        [ObservableProperty]
        private bool _hasStats;

        // ── Info overlay state ────────────────────────────────────────────────
        [ObservableProperty]
        private bool _isInfoVisible;

        [ObservableProperty]
        private string _infoTitle = string.Empty;

        [ObservableProperty]
        private string _infoDescription = string.Empty;

        [ObservableProperty]
        private string _infoEmoji = string.Empty;

        [ObservableProperty]
        private string _infoType = string.Empty;

        [ObservableProperty]
        private ObservableCollection<PermissionUsageRecord> _todayUsage = new();

        public HomeViewModel(IPrivacyRepository repository, IRefreshService refreshService, IPermissionUsageService usageService)
        {
            _repository = repository;
            _refreshService = refreshService;
            _usageService = usageService;

            _refreshService.SyncStatusChanged += OnSyncStatusChanged;
            Greeting = PrivacyLens.Helpers.GreetingHelper.GetGreeting();
        }

        private void OnSyncStatusChanged(object? sender, EventArgs e)
        {
            if (_refreshService.IsSyncing)
            {
                // Sync just started — show the loading overlay.
                MainThread.BeginInvokeOnMainThread(() => { IsRefreshing = true; IsBusy = true; });
            }
            else
            {
                // Sync finished — reload data on background thread, then update UI.
                Task.Run(async () =>
                {
                    UpdateLastUpdatedText();
                    await ReloadStatsAndActivityAsync();
                    MainThread.BeginInvokeOnMainThread(() => { IsRefreshing = false; IsBusy = false; });
                });
            }
        }

        private async Task ReloadStatsAndActivityAsync()
        {
            // All DB reads happen on a background thread.
            var (records, allApps) = await Task.Run(async () =>
            {
                var r = await _repository.GetTodayPermissionActivityAsync();
                var a = (await _repository.GetAppsAsync()).ToList();
                return (r.ToList(), a);
            });

            // One batch UI update.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HasUsageAccess = _usageService.HasUsageAccess();
                
                var appsDict = allApps.GroupBy(a => a.PackageName).ToDictionary(g => g.Key, g => g.First().IconCachePath);
                foreach (var record in records)
                {
                    if (appsDict.TryGetValue(record.PackageName, out var iconPath))
                    {
                        record.IconCachePath = iconPath;
                    }
                }

                Greeting = PrivacyLens.Helpers.GreetingHelper.GetGreeting();
                TodayUsage = new ObservableCollection<PermissionUsageRecord>(records);
                IsActivityEmpty = TodayUsage.Count == 0;

                TotalAppsTracked = allApps.Count;
                AppsWithSensitiveAccess = allApps.Count(a => a.GrantedSensitivePermissionCount > 0);
                HighRiskAppsCount = allApps.Count(a => a.GrantedSensitivePermissionCount >= 3);
                HasStats = TotalAppsTracked > 0;
            });
        }

        private void UpdateLastUpdatedText()
        {
            var lastSync = _refreshService.LastSuccessfulSyncAt;
            if (lastSync == null)
            {
                LastUpdatedText = "Never updated";
                return;
            }
            var diff = DateTimeOffset.UtcNow - lastSync.Value;
            if (diff.TotalSeconds < 60)
                LastUpdatedText = "Last updated just now";
            else if (diff.TotalMinutes < 60)
                LastUpdatedText = $"Last updated {(int)diff.TotalMinutes} minutes ago";
            else
                LastUpdatedText = $"Last updated {lastSync.Value.ToLocalTime():g}";
        }

        [RelayCommand]
        public async Task LoadDataAsync(bool checkFreshness = true)
        {
            if (IsBusy) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                await Task.Run(async () =>
                {
                    if (checkFreshness)
                        await _refreshService.EnsureFreshDataAsync();

                    UpdateLastUpdatedText();
                    await ReloadStatsAndActivityAsync();
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HasError = true;
                    ErrorMessage = "Failed to load privacy activity.";
                });
                System.Diagnostics.Debug.WriteLine($"[HomeViewModel] Error loading data: {ex}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
            }
        }

        [RelayCommand]
        public async Task ForceRefreshAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            IsRefreshing = true;
            HasError = false;
            try
            {
                await Task.Run(() => _refreshService.ForceRefreshAsync());
                UpdateLastUpdatedText();
                await ReloadStatsAndActivityAsync();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Couldn't refresh privacy data. Showing previously saved information.";
                System.Diagnostics.Debug.WriteLine($"[HomeViewModel] Force refresh error: {ex}");
            }
            finally
            {
                IsRefreshing = false;
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task NavigateToAppsAsync()
        {
            await Shell.Current.GoToAsync("//apps");
        }

        [RelayCommand]
        public async Task NavigateToSensitiveAppsAsync()
        {
            await Shell.Current.GoToAsync("filteredapps?filterType=Sensitive");
        }

        [RelayCommand]
        public async Task NavigateToHighRiskAppsAsync()
        {
            await Shell.Current.GoToAsync("filteredapps?filterType=HighRisk");
        }

        [RelayCommand]
        public async Task NavigateToPermissionsAsync()
        {
            await Shell.Current.GoToAsync("//permissions");
        }

        [RelayCommand]
        public async Task NavigateToAppDetailsAsync(PermissionUsageRecord record)
        {
            if (record == null) return;
            await Shell.Current.GoToAsync($"appdetails?packageName={record.PackageName}");
        }

        [RelayCommand]
        public void ShowAppsInfo()
        {
            InfoEmoji = "ic_phone_android.png";
            InfoType = "Apps";
            InfoTitle = "Total Apps Tracked";
            InfoDescription = "The total number of apps installed on your device that PrivacyLens has scanned and stored locally.\n\nThis includes both user-installed apps and system apps. The higher this number, the more coverage you have over your device's privacy landscape.";
            IsInfoVisible = true;
        }

        [RelayCommand]
        public void ShowSensitiveInfo()
        {
            InfoEmoji = "ic_lock_open.png";
            InfoType = "Sensitive";
            InfoTitle = "Apps with Sensitive Access";
            InfoDescription = "Apps that have been granted at least one sensitive permission — such as Camera, Microphone, Location, Contacts, or Storage.\n\nA granted permission doesn't necessarily mean the app is actively using it, but it does mean the app has the capability to access that resource at any time.";
            IsInfoVisible = true;
        }

        [RelayCommand]
        public void ShowHighRiskInfo()
        {
            InfoEmoji = "ic_warning.png";
            InfoType = "HighRisk";
            InfoTitle = "High-Risk Apps";
            InfoDescription = "Apps that hold 3 or more sensitive permissions simultaneously. These are the apps most worth reviewing.\n\nApps that combine Camera + Microphone + Location, for example, have broad access to your personal data and activity. Tap 'View Apps' to audit them in detail.";
            IsInfoVisible = true;
        }

        [RelayCommand]
        public void CloseInfo()
        {
            IsInfoVisible = false;
        }

        [RelayCommand]
        public void RequestUsageAccess()
        {
            _usageService.RequestUsageAccess();
        }
    }
}
