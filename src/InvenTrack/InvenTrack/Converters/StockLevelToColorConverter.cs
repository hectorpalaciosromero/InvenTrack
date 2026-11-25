using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InvenTrack.Converters;

public class StockLevelToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int stock || parameter is not string minStockStr)
            return Brushes.Gray;

        if (!int.TryParse(minStockStr, out int minStock))
            minStock = 10;

        return stock switch
        {
            0 => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
            _ when stock <= minStock => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
            _ => new SolidColorBrush(Color.FromRgb(46, 204, 113))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
