using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Inventory.Converters
{
    public class AndroidFileToPathValueConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            // unfornuately can't grab the android helper _dir... gotta look into that :/
            return "/storage/emulated/0/Pictures/StuffApp/" + value;
        }
    }
}
