using PrivacyLens.Services.Interfaces;
using System;
using System.Threading.Tasks;

#if ANDROID
using Android.Content;
using Android.Provider;
using Android.Net;
#endif

namespace PrivacyLens.Platforms.Android.Services
{
    public class AndroidAppSettingsService : IAppSettingsService
    {
        public Task OpenApplicationSettingsAsync(string packageName)
        {
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var intent = new Intent(global::Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.SetData(global::Android.Net.Uri.Parse($"package:{packageName}"));
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open app settings: {ex}");
            }
#endif
            return Task.CompletedTask;
        }

        public Task OpenUsageAccessSettingsAsync()
        {
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var intent = new Intent(global::Android.Provider.Settings.ActionUsageAccessSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open usage access settings: {ex}");
            }
#endif
            return Task.CompletedTask;
        }

        public Task OpenPrivacySettingsAsync()
        {
#if ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                var intent = new Intent("android.settings.PRIVACY_SETTINGS");
                intent.AddFlags(ActivityFlags.NewTask);
                
                var packageManager = context.PackageManager;
                if (intent.ResolveActivity(packageManager) != null)
                {
                    context.StartActivity(intent);
                }
                else
                {
                    var fallbackIntent = new Intent(global::Android.Provider.Settings.ActionSettings);
                    fallbackIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(fallbackIntent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open privacy settings: {ex}");
            }
#endif
            return Task.CompletedTask;
        }
    }
}
