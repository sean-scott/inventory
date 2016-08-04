using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class NegativeIntVisibilityValueConverter : MvxValueConverter<int, bool>
    {
        protected override bool Convert(int value, Type targetType, object parameter, CultureInfo culture)
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
