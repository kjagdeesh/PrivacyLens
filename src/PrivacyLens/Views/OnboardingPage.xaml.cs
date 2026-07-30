using Microsoft.Maui.Controls;
using PrivacyLens.ViewModels;

namespace PrivacyLens.Views
{
    public partial class OnboardingPage : ContentPage
    {
        public OnboardingPage(OnboardingViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
