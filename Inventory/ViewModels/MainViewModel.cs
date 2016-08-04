using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IMvxMessenger _messenger;
        private IDisposable _refreshToken;

        public MainViewModel(IDataService dataService, IMvxMessenger messenger, IBackupService oneDriveService)
            : base(dataService, messenger, oneDriveService)
        {
            _dataService = dataService;
            _messenger = messenger;
            
            _refreshToken = _messenger.Subscribe<CollectionChangedMessage>(Refresh);

            Items = _dataService.AllItems();

            _dataService.ResetAttributeStack();

            DataPaths = _dataService.GetChildPaths();
            PresentablePaths = _dataService.GetPresentablePaths(DataPaths);
        }
        
        /// <summary>
        /// Called when the current working path has been updated.
        /// </summary>
        private void Refresh(CollectionChangedMessage message)
        {
            DataPaths = _dataService.GetChildPaths();
            PresentablePaths = _dataService.GetPresentablePaths(DataPaths);
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

        private Filters _filters;
        public Filters Filters
        {
            get { return _filters; }
            set
            {
                _filters = value;
                RaisePropertyChanged(() => Filters);
            }
        }

        /// <summary>
        /// List of current available paths for attributes
        /// (e.g., "Sean's Room/Closet", etc.)
        /// </summary>
        private List<string> _presentablePaths;
        public List<string> PresentablePaths
        {
            get { return _presentablePaths; }
            set
            {
                _presentablePaths = value;
                RaisePropertyChanged(() => PresentablePaths);
            }
        }

        /// <summary>
        /// "Backend" unformatted paths for data manipulation.
        /// </summary>
        private List<string> _dataPaths;
        public List<string> DataPaths
        {
            get { return _dataPaths; }
            set
            {
                _dataPaths = value;
                RaisePropertyChanged(() => DataPaths);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        public MvxCommand SearchCommand => new MvxCommand(Search);
        void Search()
        {
            System.Diagnostics.Debug.WriteLine("MainViewModel.Search()");

            ShowViewModel<SearchViewModel>();
        }

        public MvxCommand AboutCommand => new MvxCommand(About);
        void About()
        {
            ShowViewModel<AboutViewModel>();
        }

        public MvxCommand HowToCommand => new MvxCommand(HowTo);
        void HowTo()
        {
            ShowViewModel<HowToViewModel>();
        }

        public MvxCommand ProductDefaultsCommand => new MvxCommand(EditProductDefaults);
        void EditProductDefaults()
        {
            ShowViewModel<ProductDefaultsViewModel>();
        }

        public virtual ICommand ItemSelected
        {
            get
            {
                return new MvxCommand<string>(path =>
                {
                    UpdateSpinner(path);
                });
            }
        }

        private void UpdateSpinner(string path)
        {
            System.Diagnostics.Debug.WriteLine("You selected: " + path);

            SelectedIndex = -1;

            int index;

            try
            {
                index = PresentablePaths.IndexOf(path);
            }
            catch (Exception)
            {
                index = 0;
            }

            System.Diagnostics.Debug.WriteLine("This is index: " + index);

            System.Diagnostics.Debug.WriteLine("Stack count: " + _dataService.GetAttributeStack().Count);
            System.Diagnostics.Debug.WriteLine(string.Format("Attribute Path: '{0}'", _dataService.GetAttributePath()));
            // Do nothing if we are already at top (house) level

            if (index == 0 && !string.IsNullOrEmpty(_dataService.GetAttributePath()))
            {
                System.Diagnostics.Debug.WriteLine("going up!");
                _dataService.PopAttribute();
            }

            // Do nothing if user picked same room

            else if (index >= 1 && string.IsNullOrEmpty(_dataService.GetAttributePath()))
            {
                System.Diagnostics.Debug.WriteLine("selected root room");
                System.Diagnostics.Debug.WriteLine("pushing: " + DataPaths[index]);
                _dataService.PushAttribute(DataPaths[index]);

            }

            else if (index > 1 && !string.IsNullOrEmpty(_dataService.GetAttributePath()))
            {
                System.Diagnostics.Debug.WriteLine("selected sub-room");
                System.Diagnostics.Debug.WriteLine("pushing: " + DataPaths[index]);
                _dataService.PushAttribute(DataPaths[index]);
            }

            // reset path and give it the stack's values from root to child
            _dataService.SetAttributePath("");

            string[] stackAsArray = _dataService.GetAttributeStack().ToArray();
            string tmpAttrPath = _dataService.GetAttributePath();

            for (int i = stackAsArray.Length - 1; i >= 0; i--)
            {
                tmpAttrPath += stackAsArray[i];
            }

            _dataService.SetAttributePath(tmpAttrPath);

            System.Diagnostics.Debug.WriteLine(string.Format("path to query: '{0}'", _dataService.GetAttributePath()));
            
            // Set index for view to update selection
            if (!string.IsNullOrEmpty(_dataService.GetAttributePath()))
            {
                SelectedIndex = 1;
            }
            else
            {
                SelectedIndex = 0;
            }

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);
        }
    }
}
