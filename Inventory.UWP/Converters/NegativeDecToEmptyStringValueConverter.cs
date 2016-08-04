using System;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class NegativeDecToEmptyStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dec = (decimal)value;

            if (dec < 0)
            {
                return "";
            }
            else
            {
                return dec.ToString();
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
                return decimal.Parse(str);
            }
        }
    }
}
