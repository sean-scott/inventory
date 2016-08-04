using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Content.PM;
using Android.Widget;
using Android.Graphics;

using FFImageLoading.Views;
using FFImageLoading;

using Inventory.ViewModels;

using MvvmCross.Droid.Support.V7.AppCompat;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.ViewItemActivity"
        )]
    public class ViewItemActivity : MvxAppCompatActivity<ViewItemViewModel>
    {
        Java.IO.File imageFile;

        Android.Support.Design.Widget.AppBarLayout appbar;
        ImageViewAsync imageView;

        LinearLayout container;

        protected override void OnViewModelSet()
        {
            System.Diagnostics.Debug.WriteLine("ViewItemActivity.OnViewModelSet()");
            
            // Layout
            SetContentView(Resource.Layout.activity_view_item);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // Linking up
            appbar = FindViewById<Android.Support.Design.Widget.AppBarLayout>(Resource.Id.viewitem_appbar);
            imageView = FindViewById<ImageViewAsync>(Resource.Id.viewItem_imageView);

            // Display Image
            if (!string.IsNullOrEmpty(ViewModel.Item.ImageFilename))
            {
                imageFile = new Java.IO.File(AndroidHelper._dir, ViewModel.Item.ImageFilename);

                // Display
                RunOnUiThread(() => {
                    ImageService.Instance.LoadFile(imageFile.Path).Into(imageView);
                });
            }
            else
            {
                // Hide container
                Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp =
                    (Android.Support.Design.Widget.CoordinatorLayout.LayoutParams)appbar.LayoutParameters;

                lp.Height = 0;

                appbar.LayoutParameters = lp;
            }

            container = FindViewById<LinearLayout>(Resource.Id.activity_view_item_container);

            // Get "rows"
            for (int i = 0; i < container.ChildCount; i++)
            {
                // Get imageview for row and paint it
                LinearLayout row = ((LinearLayout)container.GetChildAt(i));
                ImageView image = (ImageView)row.GetChildAt(0);
                image.SetColorFilter(Color.ParseColor(Application.Context.Resources.GetString(Resource.Color.accent)));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ImageService.Instance.InvalidateMemoryCache();
        }
    }
}