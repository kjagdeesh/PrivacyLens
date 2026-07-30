using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class AppsPage : ContentPage
    {
        public AppsPage(AppsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AppsViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
                vm.PropertyChanged += OnViewModelPropertyChanged;

                _ = vm.LoadAppsAsync(true);
            }

            // Animate page content fade in
            await ContentGrid.FadeTo(1, 350, Easing.CubicOut);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is AppsViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        private async void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppsViewModel.IsFilterPopupVisible))
            {
                var vm = (AppsViewModel)sender;
                if (vm.IsFilterPopupVisible)
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
