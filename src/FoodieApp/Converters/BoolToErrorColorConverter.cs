using System.Globalization;

namespace FoodieApp.Converters;

/// <summary>
/// Converts a boolean error flag to a border color.
/// true (has error) → Red, false (no error) → Transparent.
/// </summary>
public class BoolToErrorColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool hasError && hasError)
        {
            return Color.FromArgb("#D32F2F"); // AccessibleError red
        }

        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
