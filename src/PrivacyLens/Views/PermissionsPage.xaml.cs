using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class PermissionsPage : ContentPage
    {
        public PermissionsPage(PermissionsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is PermissionsViewModel vm)
            {
                _ = vm.LoadPermissionsAsync(true);
            }

            // Animate page content fade in
            await ContentGrid.FadeTo(1, 350, Easing.CubicOut);
        }
    }
}
