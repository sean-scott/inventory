using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class QuantityVisibilityValueConverter : MvxValueConverter<int, bool>
    {
        protected override bool Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var quantity = value;

            if (quantity > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
