using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InvenTrack.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool invert = parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase);

        bool visible = value switch
        {
            bool b => b,
            string str => !string.IsNullOrEmpty(str),
            int i => i != 0,
            null => false,
            _ => true
        };

        if (invert) visible = !visible;

        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility v && v == Visibility.Visible;
    }
}
