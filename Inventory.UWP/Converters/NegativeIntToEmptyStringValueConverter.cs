using System;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class NegativeIntToEmptyStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var quantity = (int)value;

            if (quantity < 0)
            {
                return "";
            }
            else
            {
                return quantity.ToString();
            }
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;

            if (string.IsNullOrEmpty(str))
            {
                return -1;
            }
            else
            {
                return int.Parse(str);
            }
        }
    }
}
