using System;
using System.Globalization;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Converters;
using MvvmCross.Plugins.Messenger;

namespace Inventory.Converters
{
    public class CommandToMessageValueConverter : MvxValueConverter<string, ICommand>
    {
        protected override ICommand Convert(string typeName, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxCommand(() =>
            {
                var messenger = Mvx.Resolve<IMvxMessenger>();
                var message = (MvxMessage)Activator.CreateInstance(Type.GetType(typeName), this, parameter);
                messenger.Publish(message);
            });
        }
    }
}
