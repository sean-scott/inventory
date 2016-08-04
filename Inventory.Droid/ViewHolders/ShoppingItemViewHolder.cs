using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Inventory.Droid.Activities;

namespace Inventory.Droid.ViewHolders
{
    public class ShoppingItemViewHolder : RecyclerView.ViewHolder
    {
        public CheckBox CBox { get; private set; }
        public TextView Name { get; private set; }
        public TextView Notes { get; private set; }
        public LinearLayout QuantityHolder { get; private set; }
        public ImageButton QuantityUp { get; private set; }
        public TextView Quantity { get; private set; }
        public ImageButton QuantityDown { get; private set; }

        public ShoppingItemViewHolder(View itemView, Context context) : base(itemView)
        {
            /*
            CBox = itemView.FindViewById<CheckBox>(Resource.Id.shopping_item_row_checkbox);
            Name = itemView.FindViewById<TextView>(Resource.Id.shopping_item_row_textview);
            Notes = itemView.FindViewById<TextView>(Resource.Id.shopping_item_row_textview_notes);

            QuantityHolder = itemView.FindViewById<LinearLayout>(Resource.Id.shopping_item_row_q_holder);
            QuantityUp = itemView.FindViewById<ImageButton>(Resource.Id.shopping_item_row_button_q_up);
            Quantity = itemView.FindViewById<TextView>(Resource.Id.shopping_item_row_textview_quantity);
            QuantityDown = itemView.FindViewById<ImageButton>(Resource.Id.shopping_item_row_button_q_down);
            */
            itemView.Click += delegate
            {
                if (context is MainActivity)
                {
                    //((MainActivity)context).ShoppingItemSelected(AdapterPosition);
                }
            };
        }
    }
}