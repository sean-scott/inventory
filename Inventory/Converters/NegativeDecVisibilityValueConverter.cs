using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class NegativeDecVisibilityValueConverter : MvxValueConverter<decimal, bool>
    {
        protected override bool Convert(decimal value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
