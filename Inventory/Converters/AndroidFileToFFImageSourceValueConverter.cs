using FFImageLoading.Work;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using FFImageLoading.Transformations;

namespace Inventory.Converters
{
    // another hack to get the imagesource. can't believe i couldn't do
    // this in the mvxbind... :/
    public class AndroidFileToFFImageSourceValueConverter : MvxValueConverter<string, ImageSource>
    {
        protected override ImageSource Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageSource.Filepath;
        }
    }
}
