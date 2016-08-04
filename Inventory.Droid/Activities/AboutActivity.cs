using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;
using Inventory.ViewModels;
using Android.Views;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.AboutActivity"
        )]
    public class AboutActivity : MvxAppCompatActivity<AboutViewModel>
    {
        protected override void OnViewModelSet()
        {
            // Layout
            SetContentView(Resource.Layout.activity_about);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }
    }
}