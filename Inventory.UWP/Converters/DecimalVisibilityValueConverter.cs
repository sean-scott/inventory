using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class DecimalVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (decimal)value;

            if (val < 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        // not needed
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}

