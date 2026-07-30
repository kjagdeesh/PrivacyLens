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
    public partial class AppsViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private readonly IRefreshService _refreshService;
        private List<InstalledApp> _allApps = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedFilter = "All";

        [ObservableProperty]
        private bool _isFilterPopupVisible;

        [ObservableProperty]
        private ObservableCollection<InstalledApp> _apps = new();

        public AppsViewModel(IPrivacyRepository repository, IRefreshService refreshService)
        {
            _repository = repository;
            _refreshService = refreshService;

            _refreshService.SyncStatusChanged += OnSyncStatusChanged;
        }

        private void OnSyncStatusChanged(object? sender, EventArgs e)
        {
            // Update IsRefreshing on UI thread, but kick off the load on a background thread.
            MainThread.BeginInvokeOnMainThread(() => IsRefreshing = _refreshService.IsSyncing);
            Task.Run(async () => await LoadAppsAsync(false));
        }

        [RelayCommand]
        public async Task LoadAppsAsync(bool checkFreshness = true)
        {
            if (IsBusy) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                // All heavy work runs on a background thread.
                var filtered = await Task.Run(async () =>
                {
                    if (checkFreshness)
                        await _refreshService.EnsureFreshDataAsync();

                    var appsList = await _repository.GetAppsAsync();
                    _allApps = appsList.ToList();
                    return ComputeFilteredList();
                });

                // One single UI-thread dispatch to replace the entire collection at once.
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
                    ErrorMessage = "Unable to load installed apps.";
                });
                System.Diagnostics.Debug.WriteLine($"[AppsViewModel] Error loading apps: {ex}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
            }
        }

        [RelayCommand]
        public async Task ForceRefreshAsync()
        {
            IsRefreshing = true;
            HasError = false;
            try
            {
                await _refreshService.ForceRefreshAsync();
                await LoadAppsAsync(false);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Refresh failed. Showing previously saved data.";
                System.Diagnostics.Debug.WriteLine($"[AppsViewModel] Force refresh error: {ex}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// Computes the filtered+searched list on a background thread (safe to call from any thread).
        /// </summary>
        private List<InstalledApp> ComputeFilteredList()
        {
            var filter = SelectedFilter;
            var search = SearchText;
            var filtered = _allApps.AsEnumerable();

            if (filter == "User Apps")
                filtered = filtered.Where(a => !a.IsSystemApp);
            else if (filter == "System Apps")
                filtered = filtered.Where(a => a.IsSystemApp);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var query = search.Trim();
                filtered = filtered.Where(a =>
                    a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    a.PackageName.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return filtered.ToList();
        }

        [RelayCommand]
        public void ApplyFilterAndSearch()
        {
            // Compute filter on background thread, then do a single batch UI update.
            Task.Run(() =>
            {
                var results = ComputeFilteredList();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Apps = new ObservableCollection<InstalledApp>(results);
                });
            });
        }

        [RelayCommand]
        public async Task NavigateToDetailsAsync(InstalledApp app)
        {
            if (app == null) return;
            await Shell.Current.GoToAsync($"appdetails?packageName={app.PackageName}");
        }

        [RelayCommand]
        public void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [RelayCommand]
        public void OpenFilterPopup()
        {
            IsFilterPopupVisible = true;
        }

        [RelayCommand]
        public void CloseFilterPopup()
        {
            IsFilterPopupVisible = false;
        }

        [RelayCommand]
        public void SelectFilter(string filter)
        {
            SelectedFilter = filter;
            IsFilterPopupVisible = false;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilterAndSearch();
        partial void OnSelectedFilterChanged(string value) => ApplyFilterAndSearch();
    }
}
