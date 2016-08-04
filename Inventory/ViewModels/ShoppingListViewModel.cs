using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    /// <summary>
    /// View model for list of shopping items, and related views.
    /// </summary>
    [ImplementPropertyChanged]
    public class ShoppingListViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IMvxMessenger _messenger;

        private IDisposable _upToken;
        private IDisposable _downToken;
        private IDisposable _checkedToken;
        private IDisposable _refreshToken;

        public ShoppingListViewModel(IDataService dataService, IMvxMessenger messenger, IBackupService oneDriveService)
            : base(dataService, messenger, oneDriveService)
        {
            _dataService = dataService;
            _messenger = messenger;

            // Only register message once (prevents multiple calls)
            if (_messenger.CountSubscriptionsFor<ShoppingItemQuantityUpMessage>() == 0)
            {
                _upToken = _messenger.Subscribe<ShoppingItemQuantityUpMessage>(message =>
                {
                    ShoppingItemQuantityUpCommand.Execute(message.ShoppingItem);
                });
            }

            if (_messenger.CountSubscriptionsFor<ShoppingItemQuantityDownMessage>() == 0)
            {
                _downToken = _messenger.Subscribe<ShoppingItemQuantityDownMessage>(message =>
                {
                    ShoppingItemQuantityDownCommand.Execute(message.ShoppingItem);
                });
            }

            if (_messenger.CountSubscriptionsFor<ShoppingItemCheckedMessage>() == 0)
            {
                _checkedToken = _messenger.Subscribe<ShoppingItemCheckedMessage>(message =>
                {
                    ShoppingItemCheckedCommand.Execute(message.ShoppingItem);
                });
            }

            _refreshToken = _messenger.Subscribe<CollectionChangedMessage>(Refresh);

            ShoppingItems = _dataService.AllShoppingItems();
        }

        /// <summary>
        /// Gets item count for visibility of lists, etc.
        /// </summary>
        public int Count
        {
            get { return ShoppingItems.Count; }
        }

        public ICommand CreateNewShoppingItemCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    ShowViewModel<EditShoppingItemViewModel>();
                });
            }
        }

        /// <summary>
        /// Called when an item has been added or modified.
        /// </summary>
        private void Refresh(CollectionChangedMessage message)
        {
            System.Diagnostics.Debug.WriteLine("InventoryViewModel.Refresh()");

            ShoppingItems = _dataService.AllShoppingItems();
        }

        private bool _isRefreshing;
        public virtual bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public ICommand ReloadCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    IsRefreshing = true;

                    await ReloadData();

                    IsRefreshing = false;
                });
            }
        }

        public virtual async Task ReloadData()
        {
            // By default return a completed Task
            await Task.Delay(1000);

            ShoppingItems = _dataService.AllShoppingItems();
        }

        /// <summary>
        /// List of Items in inventory.
        /// </summary>
        private List<ShoppingItem> _shoppingItems;
        public List<ShoppingItem> ShoppingItems
        {
            get { return _shoppingItems; }
            set
            {
                _shoppingItems = value;
                RaisePropertyChanged(() => ShoppingItems);
            }
        }

        private int _selectedId;
        public int SelectedId
        {
            get { return _selectedId; }
            set
            {
                _selectedId = value;
                RaisePropertyChanged(() => SelectedId);
            }
        }

        private int _selectedItemId;
        public int SelectedItemId
        {
            get { return _selectedItemId; }
            set
            {
                _selectedItemId = value;
                RaisePropertyChanged(() => SelectedItemId);
            }
        }

        public virtual ICommand ShoppingItemSelected
        {
            get
            {
                return new MvxCommand<ShoppingItem>(shoppingItem =>
                {
                    System.Diagnostics.Debug.WriteLine("ShoppingListViewModel.ShoppingItemSelected");
                    System.Diagnostics.Debug.WriteLine(shoppingItem.ToString());

                    if (shoppingItem.ItemID > 0)
                    {
                        ShowViewModel<ViewItemViewModel>(new { Id = shoppingItem.ItemID });
                    }
                    else
                    {
                        SelectedId = shoppingItem.ID;
                    }
                });
            }
        }

        /// <summary>
        /// Increments item quantity by a value of 1
        /// </summary>
        private MvxCommand<ShoppingItem> _shoppingItemQuantityUpCommand;
        public ICommand ShoppingItemQuantityUpCommand
        {
            get
            {
                _shoppingItemQuantityUpCommand = _shoppingItemQuantityUpCommand ?? new MvxCommand<ShoppingItem>(shoppingItem =>
                {
                    shoppingItem.Quantity++;
                    _dataService.SaveShoppingItem(shoppingItem);
                });

                return _shoppingItemQuantityUpCommand;
            }
        }

        /// <summary>
        /// Decrements item quantity by a value of 1 (if >0)
        /// </summary>
        private MvxCommand<ShoppingItem> _shoppingItemQuantityDownCommand;
        public ICommand ShoppingItemQuantityDownCommand
        {
            get
            {
                _shoppingItemQuantityDownCommand = _shoppingItemQuantityDownCommand ?? new MvxCommand<ShoppingItem>(shoppingItem =>
                {
                    if (shoppingItem.Quantity > 0)
                    {
                        shoppingItem.Quantity--;
                    }
                    _dataService.SaveShoppingItem(shoppingItem);
                });

                return _shoppingItemQuantityDownCommand;
            }
        }

        /// <summary>
        /// Updates CompletedSince value for shopping item
        /// </summary>
        private MvxCommand<ShoppingItem> _shoppingItemCheckedCommand;
        public ICommand ShoppingItemCheckedCommand
        {
            get
            {
                _shoppingItemCheckedCommand = _shoppingItemCheckedCommand ?? new MvxCommand<ShoppingItem>(shoppingItem =>
                {
                    if (shoppingItem.IsCompleted)
                    {
                        shoppingItem.CompletedSince = DateTime.UtcNow;

                        if (shoppingItem.ItemID > 0)
                        {
                            var item = _dataService.GetItem(shoppingItem.ItemID);
                            item.Quantity += shoppingItem.Quantity;

                            _dataService.SaveItem(item);
                        }
                    }
                    else
                    {
                        shoppingItem.CompletedSince = new DateTime();

                        if (shoppingItem.ItemID > 0)
                        {
                            var item = _dataService.GetItem(shoppingItem.ItemID);
                            item.Quantity -= shoppingItem.Quantity;

                            _dataService.SaveItem(item);
                        }
                    }
                    
                    _dataService.SaveShoppingItem(shoppingItem);
                });

                return _shoppingItemCheckedCommand;
            }
        }
    }
}
