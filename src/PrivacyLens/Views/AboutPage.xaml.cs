using Microsoft.Maui.Controls;
using PrivacyLens.ViewModels;
using System.Threading.Tasks;

namespace PrivacyLens.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage(AboutViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Global page transition animation
            await ContentGrid.FadeTo(1, 400, Easing.CubicOut);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Reset opacity for the next time the page appears
            ContentGrid.Opacity = 0;
        }
    }
}
