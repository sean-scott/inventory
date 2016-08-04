using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;
using Inventory.Resources;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class FiltersViewModel : MvxViewModel
    {
        private readonly IDataService _dataService;
        private int _initialAttribute;

        public FiltersViewModel(IDataService dataService, IMvxMessenger messenger)
        {
            _dataService = dataService;
            _messenger = messenger;

            _filters = Filters.Read();
            _initialAttribute = _filters.Attribute;
        }

        private readonly IMvxMessenger _messenger;
        public IMvxMessenger Messenger
        { get { return _messenger; } }

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

        public List<string> Attributes
        {
            get
            {
                return new List<string>()
                {
                    Strings.AttributeCategory,
                    Strings.AttributeLocation
                };
            }
        }

        public List<string> Orders
        {
            get
            {
                return new List<string>()
                {
                    Strings.OrderAscending,
                    Strings.OrderDescending
                };
            }
        }

        public List<string> Sorts
        {
            get
            {
                return new List<string>()
                {
                    Strings.SortAlphabetically,
                    Strings.SortDateModified,
                    Strings.SortDatePurchased,
                    Strings.SortRecentlyAdded,
                    Strings.SortValue
                };
            }
        }

        public MvxCommand SaveFiltersCommand => new MvxCommand(SaveFilters);
        public void SaveFilters()
        {
            System.Diagnostics.Debug.WriteLine("FiltersViewModel.SaveFilters()");

            Filters.Write(_filters.ToJson().ToString());

            // reset stack if attribute changed
            if (_filters.Attribute != _initialAttribute)
            {
                _dataService.ResetAttributeStack();
            }

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);

            Close(this);
        }
    }
}
