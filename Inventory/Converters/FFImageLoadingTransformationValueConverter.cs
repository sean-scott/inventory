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
    // a really really hacky way of getting transformation for image without modifying viewmodel
    // i'd like to do this straight in the axml, but this will do for now...
    public class FFImageLoadingTransformationValueConverter : MvxValueConverter<string, List<ITransformation>>
    {
        protected override List<ITransformation> Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return new List<ITransformation> { new CircleTransformation() };
        }
    }
}
