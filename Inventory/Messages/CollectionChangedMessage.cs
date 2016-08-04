using MvvmCross.Plugins.Messenger;

namespace Inventory.Messages
{
    public class CollectionChangedMessage : MvxMessage
    {
        public CollectionChangedMessage(object sender, bool refresh)
            : base(sender)
        {
            Refresh = refresh;
        }

        public bool Refresh { get; private set; }
    }
}
