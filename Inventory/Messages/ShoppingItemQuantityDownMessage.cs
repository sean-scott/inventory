using Inventory.Model;
using MvvmCross.Plugins.Messenger;

namespace Inventory.Messages
{
    public class ShoppingItemQuantityDownMessage : MvxMessage
    {
        public ShoppingItemQuantityDownMessage(object sender, object item)
            : base(sender)
        {
            ShoppingItem = (ShoppingItem)item;
        }

        public ShoppingItem ShoppingItem { get; set; }
    }
}
