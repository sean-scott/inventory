using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Inventory.Droid.Fragments;
using Inventory.Droid.ViewHolders;
using Inventory.Model;
using System.Collections.Generic;

namespace Inventory.Droid.Adapters
{
    public class ShoppingItemAdapter : RecyclerView.Adapter
    {
        public Context mContext;
        public List<ShoppingItem> mItems;

        public ShoppingItemAdapter(List<ShoppingItem> items, Context context)
        {
            mItems = items;
            mContext = context;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                Inflate(Resource.Layout.row_shopping_item, parent, false);
            ShoppingItemViewHolder vh = new ShoppingItemViewHolder(itemView, mContext);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ShoppingItem sItem = mItems[position];

            ShoppingItemViewHolder vh = holder as ShoppingItemViewHolder;

            vh.CBox.CheckedChange += (o, e) =>
            {
                System.Diagnostics.Debug.WriteLine("On CheckedChange...");

                //Item item = Core.GetItem(sItem.ItemID);
                Item item = new Item();
                if (vh.CBox.Checked)
                {
                    System.Diagnostics.Debug.WriteLine("Checked...");

                    if (item != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Item NOT NULL");

                        item.DatePurchased = System.DateTime.Now;

                        // Update item total
                        item.Quantity += sItem.Quantity;
                    }

                    // Prevent re-check
                    if (sItem.CompletedSince.Year == 1)
                    {
                        sItem.CompletedSince = System.DateTime.Now;
                    }

                    // Check if should convert to inventory item
                    if (sItem.MakeItem)
                    {
                        System.Diagnostics.Debug.WriteLine("Adding sItem to inventory...");

                        item = new Item();

                        item.Name = sItem.Name;
                        item.Notes = sItem.Notes;
                        item.Quantity = sItem.Quantity;
                        item.QuantityForShoppingList = 0;
                        item.DatePurchased = System.DateTime.Now;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unchecked...");

                    if (item != null)
                    {
                        item.Quantity -= sItem.Quantity;
                    }

                    sItem.CompletedSince = new System.DateTime(1, 1, 1);
                }

                if (item != null)
                {
                    //sItem.ItemID = Core.SaveItem(item);
                    //InventoryFragment.GetData();
                }

                //Core.SaveShoppingItem(sItem);

                Paint(sItem, vh);
            };

            vh.QuantityUp.Click += (o, e) =>
            {
                sItem.Quantity++;
                //Core.SaveShoppingItem(sItem);

                Paint(sItem, vh);
            };

            vh.QuantityDown.Click += (o, e) =>
            {
                if (sItem.Quantity > 1)
                {
                    sItem.Quantity--;
                    //Core.SaveShoppingItem(sItem);

                    Paint(sItem, vh);
                }
            };

            Paint(sItem, vh);
        }

        private void Paint(ShoppingItem sItem, ShoppingItemViewHolder vh)
        {
            // Check the box if it is completed
            if (sItem.CompletedSince.Year > 1)
            {
                vh.CBox.Checked = true;

                // strikethrough
                vh.Name.PaintFlags = vh.Name.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;
                vh.Name.Typeface = null;

                vh.Notes.PaintFlags = vh.Notes.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;

                // Prevent quanitity modification
                vh.QuantityUp.Visibility = ViewStates.Invisible;
                vh.QuantityDown.Visibility = ViewStates.Invisible;
            }
            else
            {
                // remove strikethrough
                vh.Name.PaintFlags = vh.Name.PaintFlags & ~Android.Graphics.PaintFlags.StrikeThruText;
                vh.Name.Typeface = Android.Graphics.Typeface.DefaultBold;

                vh.Notes.PaintFlags = vh.Notes.PaintFlags & ~Android.Graphics.PaintFlags.StrikeThruText;

                vh.QuantityUp.Visibility = ViewStates.Visible;
                vh.QuantityDown.Visibility = ViewStates.Visible;
            }

            vh.Name.Text = sItem.Name;

            if (!string.IsNullOrEmpty(sItem.Notes))
            {
                vh.Notes.Text = sItem.Notes;
            }
            else
            {
                vh.Notes.Visibility = ViewStates.Gone;
            }

            vh.Quantity.Text = sItem.Quantity.ToString();
        }

        public override int ItemCount
        {
            get
            {
                return mItems.Count;
            }
        }
    }
}