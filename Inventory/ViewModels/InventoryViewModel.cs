using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;

using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    /// <summary>
    /// View model for list of items found in the inventory, and related views.
    /// </summary>
    [ImplementPropertyChanged]
    public class InventoryViewModel
        : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IMvxMessenger _messenger;

        // When using weak references, it's important to store 
        // the returned subscription token in a class-level field.
        // http://stackoverflow.com/a/19661600
        private IDisposable _upToken;
        private IDisposable _downToken;
        private IDisposable _refreshToken;


        public InventoryViewModel(IDataService dataService, 
            IMvxMessenger messenger, 
            IBackupService oneDriveService)
            : base(dataService, messenger, oneDriveService)
        {
            _dataService = dataService;
            _messenger = messenger;

            // Only register message once (prevents multiple calls)
            if (_messenger.CountSubscriptionsFor<ItemQuantityUpMessage>() == 0)
            {
                _upToken = _messenger.Subscribe<ItemQuantityUpMessage>(message =>
                {
                    ItemQuantityUpCommand.Execute(message.Item);
                });
            }

            if (_messenger.CountSubscriptionsFor<ItemQuantityDownMessage>() == 0)
            {
                _downToken = _messenger.Subscribe<ItemQuantityDownMessage>(message =>
                {
                    ItemQuantityDownCommand.Execute(message.Item);
                });
            }

            _refreshToken = _messenger.Subscribe<CollectionChangedMessage>(Refresh);

            Items = _dataService.AllItems();
        }

        /// <summary>
        /// List of Items in inventory.
        /// </summary>
        private List<Item> _items;
        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        /// <summary>
        /// Gets item count for visibility of lists, etc.
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Called when an item has been added or modified.
        /// </summary>
        private void Refresh(CollectionChangedMessage message)
        {
            System.Diagnostics.Debug.WriteLine("InventoryViewModel.Refresh()");

            Items = _dataService.AllItems();
        }

        public virtual ICommand ItemSelected
        {
            get
            {
                return new MvxCommand<Item>(item =>
                {
                    ViewItem(item.ID);
                });
            }
        }
        public void ViewItem(int id)
        {
            ShowViewModel<ViewItemViewModel>(new { Id = id });
        }

        public void EditProduct(string json)
        {
            ShowViewModel<EditItemViewModel>(new { Product = json });
        }

        public void SaveProduct(Item product)
        {
            _dataService.SaveItem(product);
        }

        // if we had a barcode scanner interface,
        // this wouldn't need to be here :/
        public void ViewProductDefaults()
        {
            ShowViewModel<ProductDefaultsViewModel>();
        }

        public int GetItemIdForBarcode(string barcode)
        {
            return _dataService.GetItemIdForBarcode(barcode);
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

            Items = _dataService.AllItems();
        }

        /// <summary>
        /// Increments item quantity by a value of 1
        /// </summary>
        private MvxCommand<Item> _itemQuantityUpCommand;
        public ICommand ItemQuantityUpCommand
        {
            get
            {
                _itemQuantityUpCommand = _itemQuantityUpCommand ?? new MvxCommand<Item>(item =>
                {
                    item.Quantity++;
                    _dataService.SaveItem(item);
                });

                return _itemQuantityUpCommand;
            }
        }

        /// <summary>
        /// Decrements item quantity by a value of 1 (if >0)
        /// </summary>
        private MvxCommand<Item> _itemQuantityDownCommand;
        public ICommand ItemQuantityDownCommand
        {
            get
            {
                _itemQuantityDownCommand = _itemQuantityDownCommand ?? new MvxCommand<Item>(item =>
                {
                    if (item.Quantity > 0)
                    {
                        item.Quantity--;
                    }
                    _dataService.SaveItem(item);
                });

                return _itemQuantityDownCommand;
            }
        }

        /// <summary>
        /// Opens the EditItemViewModel for creating a new item.
        /// </summary>
        /*
        public ICommand CreateNewItemCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    ShowViewModel<EditItemViewModel>(new { Id = 0 });
                });
            }
        }
        */
        public MvxCommand CreateNewItemCommand => new MvxCommand(CreateNewItem);
        void CreateNewItem()
        {
            System.Diagnostics.Debug.WriteLine("COMMAND ME");
            ShowViewModel<EditItemViewModel>(new { Id = 0 });
        }

        /// <summary>
        /// One day.... we will be able to mvvm-ify zxing. one day...
        /// </summary>
        public ICommand ScanBarcodeCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    // :(
                });
            }
        }
    }
}