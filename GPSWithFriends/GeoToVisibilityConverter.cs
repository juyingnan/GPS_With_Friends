using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GPSWithFriends
{
    public class GeoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            GeoCoordinate temp = (GeoCoordinate)value;
            if (temp != null)
            {
                if (temp.Latitude > 90 || temp.Latitude < -90 || temp.Longitude > 180 || temp.Longitude < -180)
                    return System.Windows.Visibility.Collapsed;
                else return System.Windows.Visibility.Visible;
            }
            else return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
