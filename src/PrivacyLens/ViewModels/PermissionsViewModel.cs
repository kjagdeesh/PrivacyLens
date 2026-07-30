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
    public partial class PermissionsViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private readonly IRefreshService _refreshService;

        [ObservableProperty]
        private ObservableCollection<DevicePermission> _permissions = new();

        private List<DevicePermission> _allPermissions = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        public PermissionsViewModel(IPrivacyRepository repository, IRefreshService refreshService)
        {
            _repository = repository;
            _refreshService = refreshService;

            _refreshService.SyncStatusChanged += OnSyncStatusChanged;
        }

        private void OnSyncStatusChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => IsRefreshing = _refreshService.IsSyncing);
            Task.Run(async () => await LoadPermissionsAsync(false));
        }

        [RelayCommand]
        public async Task LoadPermissionsAsync(bool checkFreshness = true)
        {
            if (IsBusy) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                // Fetch + filter entirely on background thread, then do a single UI update.
                var results = await Task.Run(async () =>
                {
                    if (checkFreshness)
                        await _refreshService.EnsureFreshDataAsync();

                    _allPermissions = (await _repository.GetPermissionsAsync()).ToList();
                    return ComputeFilteredPermissions();
                });

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Permissions = new ObservableCollection<DevicePermission>(results);
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HasError = true;
                    ErrorMessage = "Failed to load device permissions.";
                });
                System.Diagnostics.Debug.WriteLine($"[PermissionsViewModel] Error: {ex}");
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
                await LoadPermissionsAsync(false);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Refresh failed. Showing previously saved data.";
                System.Diagnostics.Debug.WriteLine($"[PermissionsViewModel] Force refresh error: {ex}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task NavigateToDetailsAsync(DevicePermission permission)
        {
            if (permission == null) return;
            await Shell.Current.GoToAsync($"permissiondetails?category={permission.Category}");
        }

        [RelayCommand]
        public void ClearSearch()
        {
            SearchText = string.Empty;
        }

        /// <summary>
        /// Computes the filtered list on whichever thread calls this. Safe to call from background.
        /// </summary>
        private List<DevicePermission> ComputeFilteredPermissions()
        {
            var searchText = SearchText;
            var source = _allPermissions; // already a List, safe to enumerate from background

            if (string.IsNullOrWhiteSpace(searchText))
                return source.ToList();

            return source.Where(p =>
                p.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void ApplyFilter()
        {
            var results = ComputeFilteredPermissions();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Permissions = new ObservableCollection<DevicePermission>(results);
            });
        }
    }
}
