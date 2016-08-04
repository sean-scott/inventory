using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content.PM;
using Newtonsoft.Json.Linq;
using Android.Support.Design.Widget;
using Inventory.Droid.Fragments;
using MvvmCross.Droid.Support.V7.AppCompat;
using Inventory.ViewModels;
using Inventory.Resources;
using System.ComponentModel;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.WeakSubscription;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.ProductDefaultsActivity",
        Label = "Product Defaults"
        )]
    public class ProductDefaultsActivity : MvxAppCompatActivity<ProductDefaultsViewModel>
    {
        // would've liked to put this in the viewmodel...
        // but since we have this weird hasdate/datepurchased
        // duplicity, and with the "current date" placeholder...
        // it was simpler to do it here :P
        TextInputEditText datePurchasedEditText;
        ImageButton clearDatePurchasedImageButton;
        FloatingActionButton resetDefaultsFab;

        protected override void OnViewModelSet()
        {
            System.Diagnostics.Debug.WriteLine("ProductDefaultsActivity.OnViewModelSet()");

            // View
            SetContentView(Resource.Layout.activity_product_defaults);

            // Toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            datePurchasedEditText = FindViewById<TextInputEditText>(Resource.Id.activity_product_defaults_edittext_datepurchased);
            clearDatePurchasedImageButton = FindViewById<ImageButton>(Resource.Id.activity_product_defaults_imagebutton_clear_date);
            resetDefaultsFab = FindViewById<FloatingActionButton>(Resource.Id.activity_product_defaults_fab_reset);

            // Auto completion setup - more work than its worth in mvvm. The one time Android makes it easier...
            var locationACTV = FindViewById<AutoCompleteTextView>(Resource.Id.activity_product_defaults_autocompletetextview_location);
            var categoryACTV = FindViewById<AutoCompleteTextView>(Resource.Id.activity_product_defaults_autocompletetextview_category);

            var locAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, ViewModel.Locations);
            var catAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, ViewModel.Categories);

            locationACTV.Adapter = locAdapter;
            categoryACTV.Adapter = catAdapter;

            DisplayDate(ViewModel.Defaults.HasDate, ViewModel.Defaults.DatePurchased);
        }

        public override void OnBackPressed()
        {
            ViewModel.OnNavigateOut();
        }

        protected override void OnPause()
        {
            base.OnPause();

            clearDatePurchasedImageButton.Click -= ClearDate_Click;
            datePurchasedEditText.Click -= SelectDate_Click;
            resetDefaultsFab.Click -= ResetDefaults_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();

            clearDatePurchasedImageButton.Click += ClearDate_Click;
            datePurchasedEditText.Click += SelectDate_Click;
            resetDefaultsFab.Click += ResetDefaults_Click;
        }

        private void ClearDate_Click(object sender, EventArgs e)
        {
            DisplayDate(ViewModel.Defaults.HasDate, ViewModel.Defaults.DatePurchased);
        }

        private void SelectDate_Click(object sender, EventArgs e)
        {
            SelectDate();
        }

        private void ResetDefaults_Click(object sender, EventArgs e)
        {
            DisplayDate(ViewModel.Defaults.HasDate, ViewModel.Defaults.DatePurchased);
        }

        /// <summary>
        /// Displays DatePickerFragment instance and updates purchase date.
        /// </summary>
        private void SelectDate()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                ViewModel.Defaults.DatePurchased = new DateTime(time.Year, time.Month, time.Day);
                ViewModel.Defaults.HasDate = true;

                DisplayDate(ViewModel.Defaults.HasDate, ViewModel.Defaults.DatePurchased);
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void DisplayDate(bool hasDate, DateTime datePurchased)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("DisplayDate(hasDate:{0}, datePurchased:{1})", hasDate, datePurchased.ToShortDateString()));

            if (datePurchased.Year == 1 &&
                hasDate)
            {
                datePurchasedEditText.Text = Strings.CurrentDatePlaceholderLabel;
            }
            else if (datePurchased.Year == 1 &&
                !hasDate)
            {

                datePurchasedEditText.Text = "";
            }
            else if (datePurchased.Year > 1)
            {
                datePurchasedEditText.Text = datePurchased.ToShortDateString();
            }
        }
    }
}