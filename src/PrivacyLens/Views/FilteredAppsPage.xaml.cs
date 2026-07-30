using Microsoft.Maui.Controls;
using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class FilteredAppsPage : ContentPage
    {
        public FilteredAppsPage(FilteredAppsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Simple fade in animation
            ContentGrid.Opacity = 0;
            await ContentGrid.FadeTo(1, 400, Easing.CubicOut);
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
