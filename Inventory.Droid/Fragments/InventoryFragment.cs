using System;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using Android.Graphics;
using Inventory.Model;
using MvvmCross.Droid.Support.V4;
using Inventory.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Android.Runtime;
using ZXing.Mobile;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Inventory.Droid.Fragments
{
    //[MvxFragment(typeof (MainViewModel), Resource.Id.content_frame, true)]
    [Register("inventory.droid.fragments.InventoryFragment")]
    public class InventoryFragment : MvxFragment<InventoryViewModel>
    {
        // Xamarin failed us. We could not make ZXing a cross-platform thing
        // check nuget, maybe you can now.
        // so we gotta do it for. each. platform.
        static bool adjustingDefaults = false;

        MobileBarcodeScanner scanner;
        View zxingOverlay;
        View zxingStatusLine;
        FloatingActionButton defaultsFab;
        FloatingActionButton torchFab;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_inventory, null);

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.fragment_inventory_recycler_view);
            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;
                var layoutManager = new LinearLayoutManager(Activity);
                recyclerView.SetLayoutManager(layoutManager);
            }

            var swipeRefreshLayout = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.fragment_inventory_refresher);

            var scanBarcodeButton = view.FindViewById<Button>(Resource.Id.fragment_inventory_button_scan_barcode);
            scanBarcodeButton.Click += ScanBarcode_Click;

            var scanBarcodeFab = view.FindViewById<FloatingActionButton>(Resource.Id.fragment_inventory_fab_scan_barcode);
            scanBarcodeFab.Click += ScanBarcode_Click;

            // ZXing Scanner
            scanner = new MobileBarcodeScanner();
            scanner.UseCustomOverlay = true;

            // ZXing Overlay setup
            zxingOverlay = LayoutInflater.FromContext(Activity).Inflate(Resource.Layout.ZxingOverlay, null);
            zxingStatusLine = zxingOverlay.FindViewById<View>(Resource.Id.zxing_line);

            defaultsFab = zxingOverlay.FindViewById<FloatingActionButton>(Resource.Id.zxing_fab_defaults);
            defaultsFab.Click += DefaultsFab_Click;

            torchFab = zxingOverlay.FindViewById<FloatingActionButton>(Resource.Id.zxing_fab_torch);
            torchFab.Click += TorchFab_Click;

            scanner.CustomOverlay = zxingOverlay;

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Resume scanning
            if (adjustingDefaults)
            {
                ScanProduct();

                adjustingDefaults = false;
            }
        }

        private void DefaultsFab_Click(object sender, EventArgs e)
        {
            //StartActivityForResult(typeof(ScanTemplateActivity), AndroidHelper.RESULT_PRODUCT_DEFAULTS);
            ViewModel.ViewProductDefaults();
            scanner.Cancel();

            adjustingDefaults = true;
        }

        private void ScanBarcode_Click(object sender, EventArgs e)
        {
            ScanProduct();
        }

        private void TorchFab_Click(object sender, EventArgs e)
        {
            scanner.ToggleTorch();

            if (scanner.IsTorchOn)
            {
                torchFab.SetImageDrawable(ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_flash_off_white_24dp));
            }
            else
            {
                torchFab.SetImageDrawable(ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_flash_on_white_24dp));
            }
        }

        private async void HandleScanResult(ZXing.Result result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                scanner.PauseAnalysis();

                ScanStatus(true);

                // Check if Barcode exists
                string barcode = result.Text;

                int itemId = ViewModel.GetItemIdForBarcode(barcode);
                
                if (itemId != 0) // Barcode matches
                {
                    ViewModel.ViewItem(itemId);

                    scanner.Cancel();
                }
                else // scanned new product
                {
                    string json = (await Query.GetProductJson(barcode)).ToString();

                    try
                    {
                        JObject root = JObject.Parse(json);
                        JObject job = (JObject)root.GetValue("0");

                        // Make sure we actually have data
                        if (!((string)job.GetValue("currency")).Equals("N/A"))
                        {
                            // Get Defaults
                            ProductDefaults defaults = ProductDefaults.Read();

                            string name = (string)job.GetValue("productname");
                            string value = (string)job.GetValue("price");

                            Item product = new Item();

                            if (defaults.HasDate && defaults.DatePurchased.Year == 1)
                            {
                                product.DatePurchased = DateTime.Today;
                            }
                            else if (defaults.HasDate && defaults.DatePurchased.Year > 1)
                            {
                                product.DatePurchased = defaults.DatePurchased;
                            }
                            else if (!defaults.HasDate)
                            {
                                product.DatePurchased = new DateTime(1, 1, 1);
                            }

                            product.Barcode = barcode;
                            product.Name = name;
                            product.Quantity = defaults.Quantity;
                            product.QuantityForShoppingList = defaults.ShoppingQuantity;
                            product.Value = decimal.Parse(value);
                            product.Location = defaults.Location;
                            product.Category = defaults.Category;
                            product.Notes = defaults.Notes;

                            if (defaults.AutoAdd)
                            {
                                ViewModel.SaveProduct(product);
                            }
                            else
                            {
                                ViewModel.EditProduct(product.ToJson().ToString());
                            }

                            scanner.Cancel();

                            /*
                            // Intent setup
                            var editProduct = new Intent(this, typeof(EditItemActivity));
                            editProduct.PutExtra("type", AndroidHelper.IS_PRODUCT);

                            // Get settings for scanning
                            // Settings may have been updated between scans
                            // So that is why we check here
                            try
                            {
                                //ProductDefaults.InitializeFromJson(JObject.Parse(App.OpenJson(ProductDefaults.Filename)));
                                ProductDefaults.InitializeFromJson(JObject.Parse(await Core.GetProductDefaults()));
                            }
                            catch (Exception)
                            {
                                ProductDefaults.Initialize();
                                System.Diagnostics.Debug.WriteLine("Unable to parse JSON. File might not exist.");
                            }

                            // Enforce defaults

                            product.Quantity = ProductDefaults.Quantity;
                            product.Location = ProductDefaults.Location;
                            product.Category = ProductDefaults.Category;
                            product.Notes = ProductDefaults.Notes;


                            if (ProductDefaults.AutoAdd) // autosave
                            {
                                await Task.Run(() => Core.SaveItem(product));

                                scanner.Cancel();
                            }
                            else
                            {
                                editProduct.PutExtra("product", product.ToJson().ToString());

                                StartActivity(editProduct);

                                scanner.Cancel();
                            }
                            */
                        }
                        else
                        {
                            ScanStatus(false);
                            Activity.RunOnUiThread(() => Toast.MakeText(Activity, "Could not find product data", ToastLength.Short).Show());
                            scanner.ResumeAnalysis();
                        }
                    }
                    catch (Exception e)
                    {
                        ScanStatus(false);
                        System.Diagnostics.Debug.WriteLine("OVER HERE " + e);
                        Activity.RunOnUiThread(() => Toast.MakeText(Activity, "No result", ToastLength.Short).Show());
                        scanner.ResumeAnalysis();
                    }
                }
            }
            else
            {
                ScanStatus(false);
                Activity.RunOnUiThread(() => Toast.MakeText(Activity, "Unable to read Barcode", ToastLength.Short).Show());
            }
        }

        public void ScanProduct()
        {
            var opt = new MobileBarcodeScanningOptions();
            opt.DelayBetweenContinuousScans = 3000;

            // Reset scanner
            ScanStatus(false);

            scanner.Torch(false);
            torchFab.SetImageDrawable(ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_flash_on_white_24dp));

            // Start scanning
            scanner.ScanContinuously(opt, HandleScanResult);
        }

        private void ScanStatus(bool good)
        {
            if (good)
            {
                Activity.RunOnUiThread(() => zxingStatusLine.SetBackgroundColor(Color.Green));
            }
            else
            {
                Activity.RunOnUiThread(() => zxingStatusLine.SetBackgroundColor(Color.Red));
            }
        }
    }

    /*
    public class InventoryFragment : Android.Support.V4.App.Fragment
    {
        static Activity activity = null;
        static Context context = null;

        public static bool IsEditing = false;

        public static List<Item> items = new List<Item>();
        static SwipeRefreshLayout srl;
        public static ProgressBar progressBar;
        RecyclerView.LayoutManager layoutManager;
        static RecyclerView recyclerView;
        public static ItemAdapter itemAdapter;

        static RelativeLayout noItemsView;
        Button addItemButton;
        Button barcodeButton;
        Button oneDriveButton;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("InventoryFragment.OnCreateView()");

            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.InventoryFragment, container, false);

            srl = view.FindViewById<SwipeRefreshLayout>(Resource.Id.main_swiperefreshlayout);
            srl.SetColorSchemeColors(Resource.Color.accent);

            srl.Refresh += delegate
            {
                System.Diagnostics.Debug.WriteLine("Inventory SRL refreshing...");

                if (!IsEditing)
                {
                    GetData();
                }
                srl.Refreshing = false;
            };

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.main_progressbar_items);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.main_recyclerview);
            layoutManager = new LinearLayoutManager(context);
            recyclerView.SetLayoutManager(layoutManager);

            // might wanna remove this part
            //itemAdapter = new ItemAdapter(context, new InventoryFragment(), items);
            //recyclerView.SetAdapter(itemAdapter);

            noItemsView = view.FindViewById<RelativeLayout>(Resource.Id.inventory_no_items_view);
            addItemButton = view.FindViewById<Button>(Resource.Id.inventoryfrag_button_add_item);
            addItemButton.Background.SetColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.accent)), PorterDuff.Mode.Multiply);
            addItemButton.Click += AddItemButton_Click;

            barcodeButton = view.FindViewById<Button>(Resource.Id.inventoryfrag_button_barcode);
            barcodeButton.Background.SetColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.accent)), PorterDuff.Mode.Multiply);
            barcodeButton.Click += BarcodeButton_Click;

            oneDriveButton = view.FindViewById<Button>(Resource.Id.inventoryfrag_button_onedrive);
            oneDriveButton.Background.SetColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.onedrive)), PorterDuff.Mode.Multiply);
            oneDriveButton.Click += OneDriveButton_Click;

            //ThreadPool.QueueUserWorkItem(o => GetData());
            GetData();

            return view;
        }

        public static bool ToggleEdit()
        {
            System.Diagnostics.Debug.WriteLine("InventoryFragment.ToggleEdit()");

            IsEditing = !IsEditing;

            itemAdapter.NotifyDataSetChanged();

            try
            {
                //((MainActivity)activity).SetupInventoryFragment(IsEditing);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Unable to change edit state");
            }

            return IsEditing;
        }

    }
    */
}