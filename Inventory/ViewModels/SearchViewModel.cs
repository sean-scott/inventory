using Inventory.Interfaces;
using Inventory.Model;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace Inventory.ViewModels
{
    public class SearchViewModel : InventoryViewModel
    {
        private readonly IDataService _dataService;

        public SearchViewModel(IDataService dataService, IMvxMessenger messenger, IBackupService oneDriveService)
            : base(dataService, messenger, oneDriveService)
        {
            _dataService = dataService;
        }

        private string _query = "";
        public string Query
        {
            get { return _query; }
            set { _query = value; RaisePropertyChanged(() => Query); Items = _dataService.ItemsMatching(_query); }
        }
    }
}
