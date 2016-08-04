using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    public class DatePurchasedVisibilityValueConverter : MvxValueConverter<DateTime, bool>
    {
        protected override bool Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            var datePurchased = value;

            if (datePurchased.Year > 1)
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
