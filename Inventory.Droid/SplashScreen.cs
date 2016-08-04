using Android.App;
using MvvmCross.Droid.Views;
using Android.Content.PM;

namespace Inventory.Droid
{
    [Activity(MainLauncher = true
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.splash_screen)
        {
        }
    }
}