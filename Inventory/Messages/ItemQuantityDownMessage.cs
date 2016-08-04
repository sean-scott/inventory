using Inventory.Model;
using MvvmCross.Plugins.Messenger;

namespace Inventory.Messages
{
    public class ItemQuantityDownMessage : MvxMessage
    {
        public ItemQuantityDownMessage(object sender, object item)
            : base(sender)
        {
            Item = (Item)item;
        }

        public Item Item { get; set; }
    }
}
