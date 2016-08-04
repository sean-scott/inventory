using System;
using Windows.UI.Xaml.Data;

namespace Inventory.UWP.Converters
{
    public class DateToSimpleStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime)value;

            if (date.Year > 1)
            {
                var str = date.ToString("MM/dd/yyyy");

                return "Date Purchased: " + str;
            }

            return null;
        }

        // not needed
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
