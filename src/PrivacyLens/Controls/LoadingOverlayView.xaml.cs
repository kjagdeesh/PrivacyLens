using Microsoft.Maui.Controls;

namespace PrivacyLens.Controls
{
    public partial class LoadingOverlayView : ContentView
    {
        public static readonly BindableProperty AnimationSourceProperty =
            BindableProperty.Create(nameof(AnimationSource), typeof(string), typeof(LoadingOverlayView), "loading.json");

        public static readonly BindableProperty LoadingMessageProperty =
            BindableProperty.Create(nameof(LoadingMessage), typeof(string), typeof(LoadingOverlayView), "Loading privacy data...");

        public string AnimationSource
        {
            get => (string)GetValue(AnimationSourceProperty);
            set => SetValue(AnimationSourceProperty, value);
        }

        public string LoadingMessage
        {
            get => (string)GetValue(LoadingMessageProperty);
            set => SetValue(LoadingMessageProperty, value);
        }

        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlayView), false, propertyChanged: OnIsLoadingChanged);

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        private static async void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (LoadingOverlayView)bindable;
            var isLoading = (bool)newValue;

            if (isLoading)
            {
                view.IsVisible = true;
                await view.FadeTo(1, 150, Easing.Linear);
            }
            else
            {
                await view.FadeTo(0, 250, Easing.Linear);
                view.IsVisible = false;
            }
        }

        public LoadingOverlayView()
        {
            InitializeComponent();
        }
    }
}
