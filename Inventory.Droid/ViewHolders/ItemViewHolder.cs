using Android.Content;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Inventory.Droid.Adapters;
using System;

namespace Inventory.Droid.ViewHolders
{
    public class ItemViewHolder : RecyclerView.ViewHolder
    {
        private View view;
        
        public ImageViewAsync Image { get; private set; }
        public TextView Name { get; private set; }
        public TextView DatePurchased { get; private set; }
        public LinearLayout QuantityHolder { get; private set; }
        public ImageButton QuantityUp { get; private set; }
        public TextView Quantity { get; private set; }
        public ImageButton QuantityDown { get; private set; }
        public CheckBox EditCheckBox { get; private set; }

        public bool HasAligned { get; set; }
        public bool HasClickEvent { get; set; }

        public ItemViewHolder(View itemView, ItemAdapter adapter) : base (itemView)
        {
            view = itemView;
            /*
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.main_item_row_imageview);
            Name = itemView.FindViewById<TextView>(Resource.Id.main_item_row_textview);
            DatePurchased = itemView.FindViewById<TextView>(Resource.Id.main_item_row_textview_date);

            QuantityHolder = itemView.FindViewById<LinearLayout>(Resource.Id.main_item_row_q_holder);
            QuantityUp = itemView.FindViewById<ImageButton>(Resource.Id.main_item_row_button_q_up);
            Quantity = itemView.FindViewById<TextView>(Resource.Id.main_item_row_textview_quantity);
            QuantityDown = itemView.FindViewById<ImageButton>(Resource.Id.main_item_row_button_q_down);
            */
            HasAligned = false;
            HasClickEvent = false;

            itemView.Click += (sender, e) =>
            {
                adapter.OnClick(AdapterPosition);
            };

            itemView.LongClick += (sender, e) =>
            {
                adapter.OnLongClick(AdapterPosition);
            };
        }

        public void SetBackgroundColor(Color color)
        {
            view.SetBackgroundColor(color);
        }
    }
}