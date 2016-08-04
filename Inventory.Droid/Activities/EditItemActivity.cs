using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Provider;
using Android.Content.PM;
using Android.Graphics;
using FFImageLoading;
using FFImageLoading.Views;
using Android.Support.Design.Widget;
using Inventory.Droid.Fragments;
using Inventory.Model;
using MvvmCross.Droid.Support.V7.AppCompat;
using Inventory.ViewModels;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.EditItemActivity"
        )]
    public class EditItemActivity : MvxAppCompatActivity<EditItemViewModel>
    {
        bool hasImage = false;
        bool imageOverlayOn = false;

        Java.IO.File imageFile;
        Java.IO.File tmpImageFile;
        ImageViewAsync imageView;
        RelativeLayout imageOverlay;
        ImageButton deleteImageButton;

        TextInputEditText datePurchasedEditText;
        FloatingActionButton cameraFab;

        protected override void OnViewModelSet()
        {
            System.Diagnostics.Debug.WriteLine("EditItemActivity.OnViewModelSet()");

            SetContentView(Resource.Layout.activity_edit_item);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            if (IsThereAnAppToTakePictures())
            {
                cameraFab = FindViewById<FloatingActionButton>(Resource.Id.activity_edit_item_fab_photo);

                cameraFab.Click -= CameraFab_Click;
                cameraFab.Click += CameraFab_Click;
            }
            else
            {
                cameraFab.Hide();
            }

            imageView = FindViewById<ImageViewAsync>(Resource.Id.editItem_imageView);
            imageView.SetColorFilter(Color.ParseColor(Resources.GetString(Resource.Color.accent)));
            imageOverlay = FindViewById<RelativeLayout>(Resource.Id.edititem_relativelayout_imageoverlay);
            deleteImageButton = FindViewById<ImageButton>(Resource.Id.edititem_imagebutton_deletephoto);

            if (!string.IsNullOrEmpty(ViewModel.Item.ImageFilename))
            {
                imageFile = new Java.IO.File(AndroidHelper._dir, ViewModel.Item.ImageFilename);

                // Clear placeholder
                imageView.SetColorFilter(null);
                imageView.SetImageDrawable(null);

                LoadImage();
            }

            datePurchasedEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_date_purchased);

            // Auto completion setup - more work than its worth in mvvm. The one time Android makes it easier...
            var locationACTV = FindViewById<AutoCompleteTextView>(Resource.Id.activity_edit_item_autocompletetextview_location);
            var categoryACTV = FindViewById<AutoCompleteTextView>(Resource.Id.activity_edit_item_autocompletetextview_category);

            var locAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, ViewModel.Locations);
            var catAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, ViewModel.Categories);

            locationACTV.Adapter = locAdapter;
            categoryACTV.Adapter = catAdapter;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            System.Diagnostics.Debug.WriteLine("EditItemActivity.OnActivityResult()");

            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                System.Diagnostics.Debug.WriteLine("Result ok...");

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = imageView.Height;
                AndroidHelper.bitmap = tmpImageFile.Path.LoadAndResizeBitmap(width, height);
                if (AndroidHelper.bitmap != null)
                {
                    System.Diagnostics.Debug.WriteLine("bitmap exists...");

                    // Compress
                    FileStream fs = null;
                    AndroidHelper.DeleteImageFromDirectory(tmpImageFile.Path);
                    fs = File.Create(tmpImageFile.Path);
                    AndroidHelper.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 25, fs);
                    fs.Close();

                    // Display
                    imageView.SetColorFilter(null);
                    imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                    RunOnUiThread(() => DisplayImage(tmpImageFile.Path));

                    // In case there was a previous photo that was deleted, reset flag
                    //deletingPhoto = false;
                }
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.OnNavigatedOut();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ImageService.Instance.InvalidateMemoryCache();
        }

        protected override void OnPause()
        {
            base.OnPause();

            datePurchasedEditText.Click -= SelectDate_Click;
            deleteImageButton.Click -= DeleteImage_Click;
            imageView.Click -= ImageView_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();

            datePurchasedEditText.Click += SelectDate_Click;
            deleteImageButton.Click += DeleteImage_Click;
            imageView.Click += ImageView_Click;
        }

        private void CameraFab_Click(object sender, EventArgs e)
        {
            TakePhoto();
        }

        private void DeleteImage_Click(object sender, EventArgs e)
        {
            PrepareToDeletePhoto();
        }

        private void ImageView_Click(object sender, EventArgs e)
        {
            ToggleImageOverlay();
        }

        private void SelectDate_Click(object sender, EventArgs e)
        {
            SelectDate();
        }

        /// <summary>
        /// Displays bitmap on imageView.
        /// To be run on UI thread.
        /// </summary>
        private void DisplayImage(string filepath)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("EditItemActivity.DisplayImage(filepath: {0})", filepath));

            imageView.SetImageResource(0);

            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);

            ImageService.Instance.InvalidateMemoryCache(); // Allows for photo re-do
            ImageService.Instance.LoadFile(filepath).Into(imageView);

            hasImage = true;
        }

        /// <summary>
        /// Ensures a camera app exists on the device.
        /// </summary>
        /// <returns>If a camera app exists</returns>
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        /// <summary>
        /// Attempts to get bitmap of cached image from filepath.
        /// If file does not exist, it requests image from server.
        /// </summary>
        private void LoadImage()
        {
            System.Diagnostics.Debug.WriteLine("EditItemActivity.LoadImage()");

            if (File.Exists(imageFile.Path))
            {
                RunOnUiThread(() => {
                    DisplayImage(imageFile.Path);
                });
            }
        }

        /// <summary>
        /// Updates UI and sets flags for potential photo deletion from server.
        /// The photo is not deleted at this point, just removed from view.
        /// The user must save the item in order to make the deletion final.
        /// </summary>
        private void PrepareToDeletePhoto()
        {
            System.Diagnostics.Debug.WriteLine("EditItemActivity.PrepareToDeletePhoto()");

            //deletingPhoto = true;

            // Clear image from UI
            imageView.SetImageBitmap(null);

            // Reset imageView with placeholder
            imageView.SetColorFilter(Color.ParseColor(Resources.GetString(Resource.Color.accent)));
            imageView.SetImageDrawable(GetDrawable(Resource.Drawable.ic_visibility_off_black_24dp));
            imageView.SetScaleType(ImageView.ScaleType.Center);

            // Close out overlay
            ToggleImageOverlay();

            hasImage = false;
        }

        /// <summary>
        /// Displays DatePickerFragment instance and updates purchase date.
        /// </summary>
        private void SelectDate()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                ViewModel.Item.DatePurchased = new DateTime(time.Year, time.Month, time.Day);
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        /// <summary>
        /// Opens intent to capture a photo for the item.
        /// </summary>
        private void TakePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            ViewModel.Item.ImageFilename = string.Format("stuff_item_{0}.jpg", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            tmpImageFile = new Java.IO.File(AndroidHelper._dir, ViewModel.Item.ImageFilename);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(tmpImageFile));
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Toggles the image overlay display for deleting photo, etc. on/off.
        /// </summary>
        private void ToggleImageOverlay()
        {
            if (hasImage)
            {
                imageOverlayOn = !imageOverlayOn;

                if (imageOverlayOn)
                {
                    imageOverlay.Visibility = ViewStates.Visible;
                }
                else
                {
                    imageOverlay.Visibility = ViewStates.Gone;
                }
            }
        }
    }

    /*
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class EditItemActivity : AppCompatActivity
    {
        int[] ids;
        int type;

        Item item = new Item();

        List<string> locations = new List<string>();
        List<string> categories = new List<string>();

        Clans.Fab.FloatingActionButton cameraFab;
        Clans.Fab.FloatingActionButton saveFab;

        AppBarLayout appBarLayout;
        ProgressBar progressBar;

        TextInputLayout nameTIL;
        TextInputEditText nameEditText;
        TextInputEditText valueEditText;
        TextInputEditText quantityEditText;
        TextInputEditText quantityForShoppingListEditText;
        TextInputEditText notesEditText;

        #region Activity Lifecycle

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Layout
            SetContentView(Resource.Layout.EditItem);

            saveFab = FindViewById<Clans.Fab.FloatingActionButton>(Resource.Id.fab_itemSave);
            saveFab.Show(true);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.edititem_appbarlayout);
            progressBar = FindViewById<ProgressBar>(Resource.Id.edititem_progressbar_image);

            nameTIL = FindViewById<TextInputLayout>(Resource.Id.edititem_til_name);
            nameEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_name);
            clearDateButton = FindViewById<ImageButton>(Resource.Id.editItem_imagebutton_no_date);
            clearDateButton.SetColorFilter(Color.ParseColor(Resources.GetString(Resource.Color.accent)));

            valueEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_value);
            quantityEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_quantity);
            quantityForShoppingListEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_quantityForShoppingList);

            notesEditText = FindViewById<TextInputEditText>(Resource.Id.editItem_editText_notes);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // Get locations and categories for autocomplete
            locations = await Core.GetLocations();
            categories = await Core.GetCategories();

            // Setup adapters for autocomplete
            
            int id = Intent.GetIntExtra("id", 0);
            ids = Intent.GetIntArrayExtra("batch");
            type = Intent.GetIntExtra("type", -1);
            string productJson = Intent.GetStringExtra("product");

            if (type == AndroidHelper.IS_EDITING)
            {
                if (id != 0)
                {
                    item = Core.GetItem(id);
                }
                else
                {
                    Finish();
                }

                nameEditText.Text = item.Name;

                if (item.DatePurchased.Year > 1)
                {
                    datePurchasedEditText.Text = item.DatePurchased.ToShortDateString();
                    clearDateButton.Visibility = ViewStates.Visible;
                }

                if (decimal.Compare(item.Value, (decimal)-1.0) > 0)
                {
                    valueEditText.Text = item.Value.ToString("0.00");
                }

                if (item.Quantity != -1)
                {
                    quantityEditText.Text = item.Quantity.ToString();
                }

                if (item.QuantityForShoppingList != -1)
                {
                    quantityForShoppingListEditText.Text = item.QuantityForShoppingList.ToString();
                }

                if (!string.IsNullOrEmpty(item.Location))
                {
                    locationACTV.Text = item.Location;
                }

                if (!string.IsNullOrEmpty(item.Category))
                {
                    categoryACTV.Text = item.Category;
                }

                if (!string.IsNullOrEmpty(item.Notes))
                {
                    notesEditText.Text = item.Notes;
                }
            }
            else if (type == AndroidHelper.IS_PRODUCT)
            {
                JObject product = new JObject();

                try
                {
                    product = JObject.Parse(productJson);
                    item = new Item(product);
                }
                catch (Exception)
                {
                    product = null;
                }

                if (product != null)
                {
                    nameEditText.Text = item.Name;

                    if (item.DatePurchased.Year > 1)
                    {
                        datePurchasedEditText.Text = item.DatePurchased.ToShortDateString();
                        clearDateButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        datePurchasedEditText.Text = string.Empty;
                        clearDateButton.Visibility = ViewStates.Invisible;
                    }

                    valueEditText.Text = item.Value.ToString("0.00");

                    if (item.Quantity > -1)
                    {
                        quantityEditText.Text = item.Quantity.ToString();
                    }

                    locationACTV.Text = item.Location;

                    categoryACTV.Text = item.Category;

                    notesEditText.Text = item.Notes;
                }
            }
            else if (type == AndroidHelper.IS_BATCH)
            {
                // hey m8, there may be a day
                // and that day may never come
                // where you need to add a back
                // button.
                // you'll have to specify a minHeight
                // possibly in the xml and then this
                // could all be irrelevant.
                // whoopeee

                // hide our picture stuff
                ViewGroup.LayoutParams par = appBarLayout.LayoutParameters;
                par.Height = 0;
                par.Width = appBarLayout.Width;
                appBarLayout.LayoutParameters = par;

                cameraFab.Hide(false);

                nameTIL.Visibility = ViewStates.Gone;
            }
            else if (type == -1)
            {
                Toast.MakeText(this, "ERROR", ToastLength.Short).Show();

                Finish();
            }

            AndroidHelper.bitmap = null;
        }
        #endregion

        #region Clicks


        private void SaveFab_Click(object sender, EventArgs e)
        {
            if (type != AndroidHelper.IS_BATCH)
            {
                SaveItem(true);
            }
            else
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

                string title = string.Format("Change {0} items?", ids.Length);

                alert.SetTitle(title);
                alert.SetMessage("Empty fields will not be updated");

                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    SaveItems();
                });

                alert.SetNegativeButton("No", (senderAlert, args) => { });

                RunOnUiThread(() => alert.Show());
            }
        }

        #endregion



        /// <summary>
        /// Ensures given input for item is correct.
        /// </summary>
        /// <returns>Whether the input is valid</returns>
        private bool ValidateInput()
        {
            bool ret = true;

            item.HouseID = Core.HouseID;

            // Clear error
            if (type != AndroidHelper.IS_BATCH)
            {
                nameEditText.Error = null;

                if (string.IsNullOrEmpty(nameEditText.Text))
                {
                    nameEditText.Error = "Name cannot be blank";
                    ret = false;
                }

                if (nameEditText.Text.Length > 256)
                {
                    nameEditText.Error = "Name is too long";
                    ret = false;
                }
            }

            if (locationACTV.Text.Length > 256)
            {
                locationACTV.Error = "Location name is too long";
                ret = false;
            }

            if (categoryACTV.Text.Length > 256)
            {
                categoryACTV.Error = "Category name is too long";
                ret = false;
            }

            return ret;
        }

        private Item AssignInput(Item item)
        {
            if (type != AndroidHelper.IS_BATCH)
            {
                item.Name = nameEditText.Text;
            }

            if (this.item.DatePurchased.Year > 1)
            {
                item.DatePurchased = this.item.DatePurchased;
            }

            if (!string.IsNullOrEmpty(quantityEditText.Text))
            {
                item.Quantity = int.Parse(quantityEditText.Text);
            }

            if (!string.IsNullOrEmpty(quantityForShoppingListEditText.Text))
            {
                item.QuantityForShoppingList = int.Parse(quantityForShoppingListEditText.Text);
            }

            if (!string.IsNullOrEmpty(valueEditText.Text))
            {
                item.Value = decimal.Parse(valueEditText.Text);
            }

            if (!string.IsNullOrEmpty(locationACTV.Text))
            {
                item.Location = locationACTV.Text;
            }

            if (!string.IsNullOrEmpty(categoryACTV.Text))
            {
                item.Category = categoryACTV.Text;
            }

            if (!string.IsNullOrEmpty(notesEditText.Text))
            {
                item.Notes = notesEditText.Text;
            }

            return item;
        }

        /// <summary>
        /// Uploads Item data to server.
        /// </summary>
        /// <param Name="finish">Whether the Activity should finish</param>
        private void SaveItem(bool finish)
        {
            if (ValidateInput())
            {
                item = AssignInput(item);

                try
                {
                    if (deletingPhoto || File.Exists(tmpImageFile.Path))
                    { 
                        AndroidHelper.DeleteImageFromDirectory(imageFile.Path);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                /*
                try
                {
                    if ()
                    {
                        App.StoreImageToAppDirectory(App.bitmap, tmpImageFile.Path);

                        App.DeleteImageFromDirectory(imageFile.Path);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                *//*
                Core.SaveItem(item);
                
                if (finish)
                {
                    Finish();
                }

                // before removing server...
                //bool ret = true;
                /*
                var progressDialog = ProgressDialog.Show(this, "Please wait...", "Saving...", true);
                new Thread(new ThreadStart(async delegate
                {
                    Thread.Sleep(500);

                    byte[] imageData = null;

                    if (App.bitmap != null)
                    {
                        MemoryStream memStream = new MemoryStream();
                        App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 75, memStream);
                        imageData = memStream.ToArray();
                    }
                    
                    Core.SaveItem(item, deletingPhoto, imageData);

                    RunOnUiThread(() => progressDialog.Hide());

                    //if (ret && finish)
                    if (finish)
                    {
                        SetResult(Result.Ok);
                        Finish();
                    }

                })).Start();
                *//*
            }
        }

        private void SaveItems()
        {
            if (ValidateInput())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    Item item = Core.GetItem(ids[i]);

                    item = AssignInput(item);
                    
                    Core.SaveItem(item);
                }

                Finish();
            }
        }
    }
    */
}