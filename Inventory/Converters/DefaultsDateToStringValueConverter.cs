using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Converters
{
    public class DefaultsDateToStringValueConverter : MvxValueConverter
    {
        public string Convert(DateTime value, bool parameter, System.Globalization.CultureInfo culture)
        {
            var datePurchased = value;
            var hasDate = parameter;

            if (hasDate && datePurchased.Year == 1)
            {
                return "current date";
            }
            else if (hasDate && datePurchased.Year > 1)
            {
                return datePurchased.ToString("MM/dd/yyyy");
            }
            else if (!hasDate)
            {
                return "";
            }

            return null;
        }
    }
}
