using Inventory.Interfaces;
using Inventory.Model;
using Inventory.Resources;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class EditItemViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private string _initialItemJson;

        public EditItemViewModel(IDataService dataService, IDialogService dialogService) 
            : base(dataService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// The Item ID for reference to update.
        /// </summary>
        private int _id;
        private string _product;
        public void Init(int Id, string Product)
        {
            _id = Id;
            _product = Product;

            if (_id > 0)
            {
                _item = _dataService.GetItem(_id);
            }
            else
            {
                _item = new Item();
            }

            if (!string.IsNullOrEmpty(_product))
            {
                _item = new Item(JObject.Parse(_product));
            }

            if (string.IsNullOrEmpty(_initialItemJson))
            {
                _initialItemJson = _item.ToJson().ToString();
            }
        }
        
        /// <summary>
        /// The Item we are editing, with binding.
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
        /// Removes date from view.
        /// </summary>
        public MvxCommand ClearDateCommand => new MvxCommand(ClearDate);
        void ClearDate()
        {
            Item.DatePurchased = new DateTime();
        }

        async Task<bool> Validate()
        {
            if (string.IsNullOrEmpty(Item.Name))
            {
                await _dialogService.ShowMessage(Strings.ErrorLabel, Strings.NoNameLabel);

                return false;
            }

            if (Item.Name.Length > 256)
            {
                await _dialogService.ShowMessage(Strings.ErrorLabel, Strings.LongNameLabel);

                return false;
            }


            if (Item.Location.Length > 256)
            {
                await _dialogService.ShowMessage(Strings.ErrorLabel, Strings.LongLocationLabel);

                return false;
            }

            if (Item.Category.Length > 256)
            {
                await _dialogService.ShowMessage(Strings.ErrorLabel, Strings.LongCategoryLabel);

                return false;
            }

            return true;
        }

        public MvxCommand SaveItemCommand => new MvxCommand(SaveItem);
        async void SaveItem()
        {
            System.Diagnostics.Debug.WriteLine("EditItemViewModel.SaveItem()");
            
            if (await Validate() == false)
                return;

            _dataService.SaveItem(Item);

            Close(this);
        }

        public MvxCommand TakePictureCommand => new MvxCommand(TakePicture);
        void TakePicture()
        {
            System.Diagnostics.Debug.WriteLine("EditItemViewModel.TakePicture()");
        }

        public async Task<bool> OnNavigatedOut()
        {
            bool flag = false;

            if (_dataService.GetDiff(_initialItemJson, _item.ToJson().ToString()))
            {
                if (await _dialogService.ShowConfirmMessage(Strings.SaveChangesDialogTitle,
                    Strings.SaveChangesDialogMessage))
                {
                    SaveItem();
                }
                else
                {
                    flag = true;
                    Close(this);
                }
            }
            else
            {
                flag = true;
                Close(this);
            }

            return flag;
        }
    }
}
