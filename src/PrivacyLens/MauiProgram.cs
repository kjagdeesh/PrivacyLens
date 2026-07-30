using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace PrivacyLens
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
#if ANDROID
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
#if ANDROID
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            // Database & Repository
            builder.Services.AddSingleton<Data.PrivacyLensDatabase>();
            builder.Services.AddSingleton<Repositories.Interfaces.IPrivacyRepository, Repositories.PrivacyRepository>();

            // Services
            builder.Services.AddSingleton<Services.Interfaces.IDataSyncService, Services.DataSyncService>();
            builder.Services.AddSingleton<Services.Interfaces.IRefreshService, Services.RefreshService>();

            // Platforms Services
            builder.Services.AddSingleton<Services.Interfaces.IInstalledAppsService, Platforms.Android.Services.AndroidInstalledAppsService>();
            builder.Services.AddSingleton<Services.Interfaces.IPermissionService, Platforms.Android.Services.AndroidPermissionService>();
            builder.Services.AddSingleton<Services.Interfaces.IPermissionUsageService, Platforms.Android.Services.AndroidPermissionUsageService>();
            builder.Services.AddSingleton<Services.Interfaces.IDeviceCapabilityService, Platforms.Android.Services.AndroidDeviceCapabilityService>();
            builder.Services.AddSingleton<Services.Interfaces.IAppSettingsService, Platforms.Android.Services.AndroidAppSettingsService>();

            // ViewModels
            builder.Services.AddTransient<ViewModels.HomeViewModel>();
            builder.Services.AddTransient<ViewModels.AppsViewModel>();
            builder.Services.AddTransient<ViewModels.AppDetailsViewModel>();
            builder.Services.AddTransient<ViewModels.PermissionsViewModel>();
            builder.Services.AddTransient<ViewModels.PermissionDetailsViewModel>();
            builder.Services.AddTransient<ViewModels.AboutViewModel>();
            builder.Services.AddTransient<ViewModels.FilteredAppsViewModel>();
            builder.Services.AddTransient<ViewModels.OnboardingViewModel>();

            // Views
            builder.Services.AddTransient<Views.HomePage>();
            builder.Services.AddTransient<Views.AppsPage>();
            builder.Services.AddTransient<Views.AppDetailsPage>();
            builder.Services.AddTransient<Views.FilteredAppsPage>();
            builder.Services.AddTransient<Views.PermissionsPage>();
            builder.Services.AddTransient<Views.PermissionDetailsPage>();
            builder.Services.AddTransient<Views.AboutPage>();
            builder.Services.AddTransient<Views.OnboardingPage>();

            return builder.Build();
        }
    }
}
