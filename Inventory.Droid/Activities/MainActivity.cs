using Android.App;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;

using Inventory.Droid.Fragments;
using Inventory.Model;
using Inventory.ViewModels;

using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;

using System;
using System.Collections.Generic;
using System.IO;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using System.ComponentModel;
using MvvmCross.Platform.WeakSubscription;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.MainActivity",
        Icon = "@drawable/Icon")]
    public class MainActivity : MvxAppCompatActivity<MainViewModel>
    {
        Spinner attributeSpinner; // because the viewmodel can't reset the selection index :(

        protected override void OnViewModelSet()
        {
            System.Diagnostics.Debug.WriteLine("MainActivity.OnCreate()");

            SetContentView(Resource.Layout.activity_main);

            // Toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayUseLogoEnabled(true);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // View Pager
            var viewPager = FindViewById<ViewPager>(Resource.Id.activity_main_viewpager);
            if (viewPager != null)
            {
                var fragments = new List<MvxFragmentPagerAdapter.FragmentInfo>
                {
                    new MvxFragmentPagerAdapter.FragmentInfo("", typeof(InventoryFragment), typeof(InventoryViewModel)),
                    new MvxFragmentPagerAdapter.FragmentInfo("", typeof(ShoppingListFragment), typeof(ShoppingListViewModel)),
                    //new MvxFragmentPagerAdapter.FragmentInfo("", typeof(SettingsFragment), typeof(SettingsViewModel))
                };

                // deprecated? still works for me :P
                viewPager.Adapter = new MvxFragmentPagerAdapter(this, SupportFragmentManager, fragments);
            }

            // Tabs
            var tabLayout = FindViewById<TabLayout>(Resource.Id.activity_main_tabs);
            tabLayout.SetupWithViewPager(viewPager);

            tabLayout.GetTabAt(0).SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.ic_home_white_24dp));
            tabLayout.GetTabAt(1).SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.ic_shopping_cart_white_24dp));
            //tabLayout.GetTabAt(2).SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.ic_settings_white_24dp));

            attributeSpinner = FindViewById<Spinner>(Resource.Id.activity_main_spinner_attributes);

            // Listen for spinner update
            IMvxNotifyPropertyChanged spinnerUpdated = ViewModel as IMvxNotifyPropertyChanged;
            spinnerUpdated.WeakSubscribe(OnSpinnerUpdated);


            // Clean up old pictures
            CreateDirectoryForPictures();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            System.Diagnostics.Debug.WriteLine("MainActivity.OnCreateOptionsMenu()");

            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.main_action_sort:
                    DisplayFilterDialog();
                    return true;
                case Resource.Id.main_action_search:
                    ViewModel.SearchCommand.Execute();
                    return true;
                case Resource.Id.menu_main_action_defaults:
                    ViewModel.ProductDefaultsCommand.Execute();
                    return true;
                case Resource.Id.menu_main_action_onedrive:
                    ViewModel.OneDriveCommand.Execute();
                    return true;
                case Resource.Id.menu_main_action_about:
                    ViewModel.AboutCommand.Execute();
                    return true;
                case Resource.Id.menu_main_action_how_to:
                    ViewModel.HowToCommand.Execute();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Creates an app-specific directory for temporarily storing pictures.
        /// If the directory already exists, the existing files are purged.
        /// 
        /// This gives us the ability to store images in our own directory
        /// instead of the main photo gallery. Images are cached here until save
        /// or exit, as they will be translated to a byte array in the database.
        /// </summary>
        public void CreateDirectoryForPictures()
        {
            System.Diagnostics.Debug.WriteLine("AndroidHelper.CreateDirectoryForPictures()");

            Java.IO.File _dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "StuffApp");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
            else
            {
                // Clean up unlinked images
                List<Item> items = ViewModel.Items;
                List<string> filenames = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    if (!string.IsNullOrEmpty(items[i].ImageFilename))
                    {
                        filenames.Add(items[i].ImageFilename);
                    }
                }

                Java.IO.File[] images = _dir.ListFiles();

                try
                {
                    for (int i = images.Length - 1; i >= 0; i--)
                    {
                        if (!filenames.Contains(images[i].Name))
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("{0} does not have an associated item", images[i].Name));

                            File.Delete(images[i].Path);
                        }
                    }
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Nothing to delete");
                }

            }

            AndroidHelper._dir = _dir;

            System.Diagnostics.Debug.WriteLine("path: " + _dir.Path);
            System.Diagnostics.Debug.WriteLine("abs path: " + _dir.AbsolutePath);
        }

        private void DisplayFilterDialog()
        {
            var dialog = new FiltersDialogFragment();
            dialog.ViewModel = new FiltersViewModel(ViewModel.DataService, ViewModel.Messenger);
            dialog.Show(SupportFragmentManager, "");
        }

        private void OnSpinnerUpdated(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MainActivity.OnSpinnerUpdated");

            if (e.PropertyName == "SelectedIndex")
            {
                System.Diagnostics.Debug.WriteLine("SelectedIndex: " + ViewModel.SelectedIndex);

                if (ViewModel.SelectedIndex > -1)
                {
                    attributeSpinner.SetSelection(ViewModel.SelectedIndex);
                }
            }
        }
    }

    /*

        #region Overrides

        #endregion

        #region Floating Action Buttons
        
        private void DeleteItemsFab_Click(object sender, EventArgs e)
        {
            List<Item> itemsToDelete = ItemAdapter.selectedItems;

            if (itemsToDelete.Count > 0)
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

                string title = string.Format("Delete {0} items?", itemsToDelete.Count);

                alert.SetTitle(title);
                alert.SetMessage("This cannot be undone");

                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    DeleteItems(itemsToDelete);
                });

                alert.SetNegativeButton("No", (senderAlert, args) => { });

                RunOnUiThread(() => alert.Show());
            }
        }


        private void EditItemsFab_Click(object sender, EventArgs e)
        {
            List<Item> itemsToEdit = ItemAdapter.selectedItems;

            if (itemsToEdit.Count > 0)
            {
                EditItems(itemsToEdit);
            }
        }

        #endregion

        #region UI Helpers
        
    
        
    
        private void DeleteItems(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Core.DeleteItem(items[i]);
            }

            RefreshData();
        }

        private void EditItems(List<Item> items)
        {
            var editItemsActivity = new Intent(this, typeof(EditItemActivity));

            int[] ids = new int[items.Count];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = items[i].ID;
            }

            if (ids.Length > 1)
            {
                editItemsActivity.PutExtra("type", AndroidHelper.IS_BATCH);
                editItemsActivity.PutExtra("batch", ids);
            }
            else
            {
                editItemsActivity.PutExtra("type", AndroidHelper.IS_EDITING);
                editItemsActivity.PutExtra("id", ids[0]);
            }

            StartActivity(editItemsActivity);
        }
        
        public void ItemSelected(int index)
        {
            if (!InventoryFragment.IsEditing)
            {
                ViewItem(InventoryFragment.items[index].ID);
            }
            else
            {
                InventoryFragment.itemAdapter.NotifyDataSetChanged();
            }
        }
        

    }
    */
}


