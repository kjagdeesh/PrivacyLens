using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class HomePage : ContentPage
    {
        private bool _isFirstLoad = true;

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
            // Workaround for MAUI Shell bug where OnAppearing is swallowed when 
            // the root page is swapped dynamically (e.g. after Onboarding).
            this.Loaded += async (s, e) =>
            {
                if (_isFirstLoad && BindingContext is HomeViewModel vm)
                {
                    _isFirstLoad = false;
                    _ = vm.LoadDataAsync(true);
                    await ContentGrid.FadeTo(1, 350, Easing.CubicOut);
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HomeViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
                vm.PropertyChanged += OnViewModelPropertyChanged;

                if (!_isFirstLoad)
                {
                    _ = vm.LoadDataAsync(true);
                }
            }

            // Animate page content fade in if it hasn't already
            if (!_isFirstLoad)
            {
                await ContentGrid.FadeTo(1, 350, Easing.CubicOut);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is HomeViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        private async void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.IsInfoVisible))
            {
                var vm = (HomeViewModel)sender;
                if (vm.IsInfoVisible)
                {
                    BottomSheetGrid.IsVisible = true;
                    BottomSheetGrid.InputTransparent = false;
                    
                    // Animate backdrop and sheet
                    var fadeTask = BottomSheetBackdrop.FadeTo(1, 250, Easing.Linear);
                    var slideTask = BottomSheetContent.TranslateTo(0, 0, 350, Easing.CubicOut);
                    await Task.WhenAll(fadeTask, slideTask);
                }
                else
                {
                    BottomSheetGrid.InputTransparent = true;

                    // Animate closing
                    var fadeTask = BottomSheetBackdrop.FadeTo(0, 200, Easing.Linear);
                    var slideTask = BottomSheetContent.TranslateTo(0, BottomSheetContent.Height > 0 ? BottomSheetContent.Height : 600, 250, Easing.CubicIn);
                    await Task.WhenAll(fadeTask, slideTask);

                    BottomSheetGrid.IsVisible = false;
                }
            }
        }
    }
}
