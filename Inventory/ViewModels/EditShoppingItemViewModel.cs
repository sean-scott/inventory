using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace Inventory.ViewModels
{
    public class EditShoppingItemViewModel : MvxViewModel
    {
        private readonly IDataService _dataService;

        public EditShoppingItemViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        /// <summary>
        /// The Item ID for reference to update.
        /// </summary>
        private int _id;
        public void Init(int Id)
        {
            _id = Id;

            if (_id > 0)
            {
                _shoppingItem = _dataService.GetShoppingItem(_id);
            }
            else
            {
                _shoppingItem = new ShoppingItem();
            }
        }

        /// <summary>
        /// Tells us whether we are editing an existing item or creating a new one.
        /// </summary>
        private bool _isNew;
        public bool IsNew
        {
            get { return _id == 0; }
            set
            {
                _isNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }
        bool Validate()
        {
            if (string.IsNullOrEmpty(ShoppingItem.Name))
                return false;

            return true;
        }

        public MvxCommand SaveShoppingItemCommand => new MvxCommand(SaveShoppingItem);
        public void SaveShoppingItem()
        {
            System.Diagnostics.Debug.WriteLine("EditShoppingItemViewModel.SaveItem()");

            // display error?
            if (!Validate())
                return;

            _dataService.SaveShoppingItem(ShoppingItem);

            Close(this);
        }

        public MvxCommand DeleteShoppingItemCommand => new MvxCommand(DeleteShoppingItem);
        public void DeleteShoppingItem()
        {
            _dataService.DeleteShoppingItem(_id);

            Close(this);
        }

        /// <summary>
        /// The ShoppingItem we are editing, with binding.
        /// </summary>
        private ShoppingItem _shoppingItem;
        public ShoppingItem ShoppingItem
        {
            get { return _shoppingItem; }
            set
            {
                _shoppingItem = value;
                RaisePropertyChanged(() => ShoppingItem);
            }
        }
    }
}
