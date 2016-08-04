using Android.Views;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Inventory.Converters
{
    /// <summary>
    /// Inverse, visibility
    /// </summary>
    public class BoolToAndroidVisibilityValueConverter : MvxValueConverter<bool, ViewStates>
    {
        protected override ViewStates Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value)
            {
                return ViewStates.Invisible;
            }
            else
            {
                return ViewStates.Visible;
            }
        }
    }
}
