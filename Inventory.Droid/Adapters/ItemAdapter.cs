using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using Inventory.Droid.Activities;
using Inventory.Droid.Fragments;
using Inventory.Droid.ViewHolders;
using Inventory.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Droid.Adapters
{
    public class ItemAdapter : RecyclerView.Adapter
    {

        public Context context = null;
        public Fragment fragment = null;
        public List<Item> items;

        public static List<int> selectedPositions = new List<int>();
        public static List<Item> selectedItems = new List<Item>();
    
        public ItemAdapter(Context context, Fragment fragment, List<Item> items)
        {
            this.context = context;
            this.fragment = fragment;
            this.items = items;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                Inflate(Resource.Layout.row_item, parent, false);

            ItemViewHolder vh = new ItemViewHolder(itemView, this);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.OnBindViewHolder()");

            Item item = items[position];

            ItemViewHolder vh = holder as ItemViewHolder;

            vh.Name.Text = item.Name;

            if (item.DatePurchased.Year > 1)
            {
                vh.DatePurchased.Text = string.Format("Purchased on " + item.DatePurchased.ToShortDateString());
            }
            else
            {
                vh.DatePurchased.Visibility = ViewStates.Gone;
            }
            /*
            if ((fragment != null && !InventoryFragment.IsEditing) || fragment == null)
            {
                selectedPositions.Clear();
                selectedItems.Clear();

                DisplayImage(vh, item);
                FormatCell(vh, false);

                vh.QuantityUp.Alpha = 1;
                vh.QuantityDown.Alpha = 1;

                if (item.Quantity > -1)
                {
                    vh.QuantityHolder.Visibility = ViewStates.Visible;
                    vh.QuantityHolder.Alpha = 1;
                    
                    // Prevents clicks being fired multiple times following
                    // a notifydatasetchanged(), etc.
                    if (!vh.HasClickEvent)
                    {
                        vh.QuantityUp.Click += (o, e) =>
                        {
                            item.Quantity++;
                            vh.Quantity.Text = item.Quantity.ToString();

                            SaveItemAndUpdateShoppingList(item);
                        };

                        vh.QuantityDown.Click += (o, e) =>
                        {
                            if (item.Quantity > 0)
                            {
                                item.Quantity--;
                                vh.Quantity.Text = item.Quantity.ToString();

                                SaveItemAndUpdateShoppingList(item);
                            }
                        };

                        vh.HasClickEvent = true;
                    }

                    vh.Quantity.Text = item.Quantity.ToString();
                }
                else
                {
                    vh.QuantityHolder.Visibility = ViewStates.Gone;
                }
            }
            else if (fragment != null && InventoryFragment.IsEditing)
            {
                if (item.Quantity > -1)
                {
                    vh.QuantityUp.Alpha = 0;
                    vh.QuantityDown.Alpha = 0;
                }
                else
                {
                    vh.QuantityHolder.Visibility = ViewStates.Gone;
                }

                if (selectedPositions.Contains(position))
                {
                    FormatCell(vh, true);
                }
                else
                {
                    FormatCell(vh, false);
                }
            }
            */
        }

        private void SaveItemAndUpdateShoppingList(Item item)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.SaveItemAndUpdateShoppingList()");

            //await Task.Run(() => Core.SaveItem(item));

            System.Diagnostics.Debug.WriteLine("item q for shop list: " + item.QuantityForShoppingList);

            if (item.QuantityForShoppingList > -1)
            {
                //ShoppingListFragment.GetData();
            }
        }

        private async void DisplayImage(ItemViewHolder vh, Item item)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.DisplayImage()");

            if (!string.IsNullOrEmpty(item.ImageFilename))
            {
                Java.IO.File imageFile = new Java.IO.File(AndroidHelper._dir, item.ImageFilename);

                // Display
                try
                {
                    await ImageService.Instance.LoadFile(imageFile.Path).DownSampleInDip(48).Transform(new CircleTransformation()).IntoAsync(vh.Image);

                    /* might be useful...
                     * ImageService.Instance.LoadUrl(urlToImage)
				.Retry(3, 200)
				.DownSample(300, 300)
				.Transform(new CircleTransformation())
				.Transform(new GrayscaleTransformation())
				.LoadingPlaceholder(Config.LoadingPlaceholderPath, ImageSource.ApplicationBundle) <----
				.ErrorPlaceholder(Config.ErrorPlaceholderPath, ImageSource.ApplicationBundle) <----
				.Into(_imgDisplay);
                     */
                    vh.Image.Visibility = ViewStates.Visible;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
            else
            {
                vh.Image.Visibility = ViewStates.Gone;

                if (!vh.HasAligned)
                {
                    // push quantityholder over dp width of image + marginleft + marginright
                    LinearLayout.LayoutParams iPar = (LinearLayout.LayoutParams)vh.Image.LayoutParameters;
                    int shift = iPar.LeftMargin + iPar.Width + iPar.RightMargin;

                    LinearLayout.LayoutParams qPar = (LinearLayout.LayoutParams)vh.QuantityHolder.LayoutParameters;

                    qPar.LeftMargin += shift;

                    vh.QuantityHolder.LayoutParameters = qPar;

                    vh.HasAligned = true;
                }

            }
        }

        private void FormatCell(ItemViewHolder vh, bool selected)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.FormatCell()");

            if (selected)
            {
                vh.SetBackgroundColor(new Color(ContextCompat.GetColor(context, Resource.Color.accent)));
                vh.Name.SetTextColor(Color.White);
                vh.DatePurchased.SetTextColor(Color.White);
                vh.Quantity.SetTextColor(Color.White);
            }
            else
            {
                vh.SetBackgroundColor(Color.White);
                vh.Name.SetTextColor(Color.Black);
                vh.DatePurchased.SetTextColor(Color.Black);
                vh.Quantity.SetTextColor(Color.Black);
            }
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }

        public void OnClick(int position)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.OnClick()");

            if (fragment != null)
            {
                if (fragment is InventoryFragment)
                {
                    /*
                    if (InventoryFragment.IsEditing)
                    {
                        if (!selectedPositions.Contains(position))
                        {
                            try
                            {
                                selectedPositions.Add(position);
                                selectedItems.Add(items[position]);
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e);
                            }
                        }
                        else
                        {
                            try
                            {
                                selectedPositions.Remove(position);
                                selectedItems.Remove(items[position]);
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e);
                            }
                        }
                    }
                    */
                }
            }

            if (context is MainActivity)
            {
                //((MainActivity)context).ItemSelected(position);
            }
            else if (context is SearchActivity)
            {
                //((SearchActivity)context).ItemSelected(position);
            }
        }

        public void OnLongClick(int position)
        {
            System.Diagnostics.Debug.WriteLine("ItemAdapter.OnLongClick()");

            if (fragment != null)
            {
                if (fragment is InventoryFragment)
                {
                    /*
                    if (InventoryFragment.ToggleEdit())
                    {
                        if (!selectedPositions.Contains(position))
                        {
                            selectedPositions.Add(position);
                            selectedItems.Add(items[position]);
                        }
                    }
                    */
                }
            }
        }
    }
}