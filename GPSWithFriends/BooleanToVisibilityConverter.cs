using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GPSWithFriends
{
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                return Visibility.Collapsed;
            if (!(value is bool))
                return Visibility.Collapsed;
            else
            {
                if (value == null)
                    return Visibility.Collapsed;
                else
                {
                    if (!(bool)value)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
