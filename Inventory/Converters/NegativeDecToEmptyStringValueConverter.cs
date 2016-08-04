using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class NegativeDecToEmptyStringValueConverter : MvxValueConverter<decimal, string>
    {
        protected override string Convert(decimal value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value < 0)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        protected override decimal ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }
            else
            {
                return decimal.Parse(value);
            }
        }
    }
}
