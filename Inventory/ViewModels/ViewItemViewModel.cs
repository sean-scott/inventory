using Inventory.Interfaces;
using Inventory.Messages;
using Inventory.Model;
using Inventory.Resources;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using PropertyChanged;

namespace Inventory.ViewModels
{
    //[ImplementPropertyChanged]
    public class ViewItemViewModel : MvxViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        public ViewItemViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
        }

        private int _id;
        public void Init(int Id)
        {
            _id = Id;

            if (_id > 0)
            {
                _item = _dataService.GetItem(_id);
            }
            else
            {
                // We should always be viewing an existing item.
                Close(this);
            }
        }

        /// <summary>
        /// The Item we are viewing, with binding.
        /// </summary>
        private Item _item;
        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                RaisePropertyChanged(() => Item);
            }
        }

        /// <summary>
        /// Deletes the currently viewed item from the database.
        /// </summary>
        public MvxCommand DeleteCommand => new MvxCommand(DeleteItem);
        private async void DeleteItem()
        {

            //if (await _dialogService.ShowConfirmMessage(Strings.DeleteItemDialogTitle,
            //    Strings.DeleteItemDialogMessage))
            if (await _dialogService.ShowConfirmMessage("Delete Item?", "This cannot be undone"))
            {
                _dataService.DeleteItem(_id);

                Close(this);
            }
        }

        /// <summary>
        /// Opens edit view to modify currently viewed item.
        /// </summary>
        public MvxCommand EditCommand => new MvxCommand(EditItem);
        private void EditItem()
        {
            ShowViewModel<EditItemViewModel>(new { Id = _id });
            Close(this);
        }
    }

}
