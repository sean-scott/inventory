using System;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class DateToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var datePurchased = (DateTime)value;

            if (datePurchased.Year > 1)
            {
                return datePurchased.ToString("MM/dd/yyyy");
            }

            return null;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;

            if (string.IsNullOrEmpty(str))
            {
                return new DateTime();
            }
            else
            {
                return DateTime.Parse(str);
            }
        }
    }
}
