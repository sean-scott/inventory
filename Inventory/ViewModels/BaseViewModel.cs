using Inventory.Interfaces;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        private readonly IDataService _dataService;
        public IDataService DataService
        {
            get { return _dataService; }
        }

        private readonly IMvxMessenger _messenger;
        public IMvxMessenger Messenger
        {
            get { return _messenger; }
        }

        private readonly IBackupService _oneDriveService;

        public BaseViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        public BaseViewModel(IDataService dataService, IMvxMessenger messenger, IBackupService oneDriveService)
        {
            _dataService = dataService;
            _messenger = messenger;
            _oneDriveService = oneDriveService;
        }

        /// <summary>
        /// List of categories for autocomplete.
        /// </summary>
        public List<string> Categories
        {
            get { return _dataService.GetCategories(); }
        }

        /// <summary>
        /// List of locations for autocomplete.
        /// </summary>
        public List<string> Locations
        {
            get { return _dataService.GetLocations(); }
        }

        /// <summary>
        /// Signs in/syncs with OneDrive
        /// </summary>
        public MvxCommand OneDriveCommand => new MvxCommand(OneDrive);
        async void OneDrive()
        {
            await _oneDriveService.Login();
        }
    }
}
