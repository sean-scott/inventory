using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class QuantityVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var quantity = (int)value;

            if (quantity > -1)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        // not needed
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
