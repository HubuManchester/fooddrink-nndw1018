using System.Globalization;

namespace FoodieApp.Converters;

public class ScanButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isScanning && isScanning)
        {
            return "Stop Scanning";
        }

        return "Start Scanning";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
