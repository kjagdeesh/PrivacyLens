using System.Globalization;

namespace PrivacyLens.Converters
{
    public class OnboardingButtonTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int position)
            {
                // We have 4 slides (0, 1, 2, 3). If position == 3, show "Get Started", else "Next"
                return position == 3 ? "Get Started" : "Next";
            }
            return "Next";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
