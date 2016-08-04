using Inventory.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Inventory
{
    public class AppStart : MvxNavigatingObject, IMvxAppStart
    {

        /// <summary>
        /// Start is called on startup of the app
        /// Hint contains information in case the app is started with extra parameters
        /// </summary>
        public void Start(object hint = null)
        {
            ShowViewModel<MainViewModel>();

            /*
            Mvx.Resolve<IAutobackupManager>().RestoreBackupIfNewer();

            Mvx.Resolve<IRecurringPaymentManager>().CheckRecurringPayments();
            Mvx.Resolve<IPaymentManager>().ClearPayments();

            // Do the first navigation
            ShowViewModel<MainViewModel>();
            */
        }
    }
}