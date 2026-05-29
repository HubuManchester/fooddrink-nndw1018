using System.Globalization;

namespace FoodieApp.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isTrue && isTrue)
        {
            return Color.FromArgb("#FF6B6B");
        }

        return Color.FromArgb("#CCCCCC");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
