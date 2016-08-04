using Inventory.Resources;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    /// <summary>
    /// Returns a user-friendly formatted DatePurchased value.
    /// </summary>
    public class DatePurchasedToStringValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            var datePurchased = value;

            if (datePurchased.Year > 1)
            {
                return Strings.DatePurchasedLabel + " " + datePurchased.ToString("MM/dd/yyyy") + " "; // extra space because italics
            }

            return null;
        }
    }
}
