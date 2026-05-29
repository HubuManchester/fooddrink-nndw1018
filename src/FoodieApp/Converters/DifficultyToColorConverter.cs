using System.Globalization;

namespace FoodieApp.Converters;

public class DifficultyToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string difficulty)
        {
            return difficulty.ToLowerInvariant() switch
            {
                "easy" => Color.FromArgb("#4CAF50"),
                "medium" => Color.FromArgb("#FF9800"),
                "hard" => Color.FromArgb("#F44336"),
                _ => Color.FromArgb("#9E9E9E")
            };
        }

        return Color.FromArgb("#9E9E9E");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
