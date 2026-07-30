using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrivacyLens.Enums;
using PrivacyLens.Models;
using PrivacyLens.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyLens.ViewModels
{
    [QueryProperty(nameof(CategoryString), "category")]
    public partial class PermissionDetailsViewModel : BaseViewModel
    {
        private readonly IPrivacyRepository _repository;
        private string _categoryString = string.Empty;
        
        [ObservableProperty]
        private PermissionCategory _category;

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private int _grantedAppCount;

        [ObservableProperty]
        private ObservableCollection<PermissionAccessItem> _appsWithAccess = new();

        public string CategoryString
        {
            get => _categoryString;
            set
            {
                _categoryString = value;
                if (Enum.TryParse<PermissionCategory>(value, out var cat))
                {
                    Category = cat;
                    DisplayName = cat.ToString();
                    Task.Run(async () => await LoadDetailsAsync());
                }
            }
        }

        public PermissionDetailsViewModel(IPrivacyRepository repository)
        {
            _repository = repository;
        }

        public async Task LoadDetailsAsync()
        {
            if (IsBusy) return;

            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; HasError = false; });
            try
            {
                var items = await Task.Run(() => _repository.GetPermissionAccessItemsAsync(Category));

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AppsWithAccess = new ObservableCollection<PermissionAccessItem>(items);
                    GrantedAppCount = items.Count;
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HasError = true;
                    ErrorMessage = "Failed to load permission details.";
                });
                System.Diagnostics.Debug.WriteLine($"[PermissionDetailsViewModel] Error loading details: {ex}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
            }
        }

        [RelayCommand]
        public async Task NavigateToAppDetailsAsync(PermissionAccessItem item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"appdetails?packageName={item.PackageName}");
        }
    }
}
