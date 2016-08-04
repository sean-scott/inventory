using Inventory.Interfaces;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;
using MvvmCross.WindowsUWP.Platform;
using Windows.UI.Xaml.Controls;

using PluginLoader = MvvmCross.Plugins.Messenger.PluginLoader;

namespace Inventory.UWP
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        public override void LoadPlugins(IMvxPluginManager pluginManager)
        {
            base.LoadPlugins(pluginManager);
            pluginManager.EnsurePluginLoaded<PluginLoader>();

            // loading via bootloader won't work for UWP projects?
        }

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();

            Mvx.RegisterType<IDialogService, DialogService>();
            Mvx.RegisterType<IStoreFeatures, StoreFeatures>();
            Mvx.RegisterType<IOneDriveAuthenticator, OneDriveAuthenticator>();
        }

        protected override IMvxApplication CreateApp()
        {
            return new Inventory.App();
        }
    }
}
