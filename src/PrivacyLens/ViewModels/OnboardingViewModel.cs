using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PrivacyLens.ViewModels
{
    public class OnboardingSlide
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty;
    }

    public partial class OnboardingViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<OnboardingSlide> _slides = new();

        [ObservableProperty]
        private int _position;

        public OnboardingViewModel()
        {
            Slides = new ObservableCollection<OnboardingSlide>
            {
                new OnboardingSlide
                {
                    Title = "Welcome to PrivacyLens",
                    Description = "See exactly what your apps are doing behind the scenes. Gain complete transparency and take control of your personal data.",
                    ImageSource = "onboarding_1.png"
                },
                new OnboardingSlide
                {
                    Title = "Monitor Permissions",
                    Description = "Track which apps have access to sensitive permissions like your Camera, Microphone, Location, and Contacts.",
                    ImageSource = "onboarding_2.png"
                },
                new OnboardingSlide
                {
                    Title = "Identify High Risks",
                    Description = "Automatically scan for apps with overly broad access to your personal data, so you can review and revoke unnecessary permissions.",
                    ImageSource = "onboarding_3.png"
                },
                new OnboardingSlide
                {
                    Title = "Real-time Activity",
                    Description = "View a chronological timeline of when and how apps are accessing your data in the background.",
                    ImageSource = "onboarding_4.png"
                }
            };
        }

        [RelayCommand]
        private void Next()
        {
            if (Position < Slides.Count - 1)
            {
                Position++;
            }
            else
            {
                GetStarted();
            }
        }

        [RelayCommand]
        private void Skip()
        {
            GetStarted();
        }

        [RelayCommand]
        private void GetStarted()
        {
            Preferences.Default.Set("IsFirstLaunch", false);
            Application.Current!.Windows[0].Page = new AppShell();
        }
    }
}
