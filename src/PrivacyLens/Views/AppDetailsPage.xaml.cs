using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class AppDetailsPage : ContentPage
    {
        public AppDetailsPage(AppDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ContentGrid.FadeTo(1, 350, Easing.CubicOut);
        }
    }
}
