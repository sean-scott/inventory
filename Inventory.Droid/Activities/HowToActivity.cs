using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;
using Inventory.ViewModels;
using Android.Views;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.HowToActivity"
        )]
    public class HowToActivity : MvxAppCompatActivity<HowToViewModel>
    {
        protected override void OnViewModelSet()
        {
            // Layout
            SetContentView(Resource.Layout.activity_how_to);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }
    }
}