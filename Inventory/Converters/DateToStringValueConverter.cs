using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace Inventory.Converters
{
    /// <summary>
    /// Returns a simple string value if date's year > 1.
    /// </summary>
    public class DateToStringValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            var datePurchased = value;

            if (datePurchased.Year > 1)
            {
                return datePurchased.ToString("MM/dd/yyyy");
            }

            return null;
        }
    }
}
