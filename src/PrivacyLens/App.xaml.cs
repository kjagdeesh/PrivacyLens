using Microsoft.Extensions.DependencyInjection;

namespace PrivacyLens
{
    public partial class App : Application
    {
        private readonly Services.Interfaces.IRefreshService _refreshService;

        public App(Services.Interfaces.IRefreshService refreshService)
        {
            InitializeComponent();
            _refreshService = refreshService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var isFirstLaunch = Preferences.Default.Get("IsFirstLaunch", true);

            if (isFirstLaunch)
            {
                var onboardingViewModel = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<ViewModels.OnboardingViewModel>();
                return new Window(new Views.OnboardingPage(onboardingViewModel));
            }

            return new Window(new AppShell());
        }

        protected override void OnResume()
        {
            base.OnResume();
            _ = _refreshService.EnsureFreshDataAsync();
        }
    }
}