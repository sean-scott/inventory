using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class NegativeIntToEmptyStringValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
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

        protected override int ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }
            else
            {
                return int.Parse(value);
            }
        }
    }
}
