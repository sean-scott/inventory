using Inventory.Interfaces;
using Inventory.Model;
using Inventory.Resources;
using MvvmCross.Core.ViewModels;
using PropertyChanged;
using System;

namespace Inventory.ViewModels
{
    [ImplementPropertyChanged]
    public class ProductDefaultsViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private string _initialDefaultsJson;

        public ProductDefaultsViewModel(IDataService dataService, IDialogService dialogService) 
            : base(dataService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            _defaults = ProductDefaults.Read();

            if (string.IsNullOrEmpty(_initialDefaultsJson))
            {
                _initialDefaultsJson = _defaults.ToJson().ToString();
            }
        }

        /// <summary>
        /// The Product Defaults we are viewing.
        /// </summary>
        private ProductDefaults _defaults;
        public ProductDefaults Defaults
        {
            get { return _defaults; }
            set
            {
                _defaults = value;
                RaisePropertyChanged(() => Defaults);
            }
        }

        /// <summary>
        /// Removes date.
        /// </summary>
        public MvxCommand ClearDateCommand => new MvxCommand(ClearDate);
        void ClearDate()
        {
            Defaults.DatePurchased = new DateTime();
            Defaults.HasDate = false;
        }

        /// <summary>
        /// Resets Defaults to their original values.
        /// </summary>
        public MvxCommand ResetDefaultsCommand => new MvxCommand(ResetDefaults);
        void ResetDefaults()
        {
            System.Diagnostics.Debug.WriteLine("ProductDefaultsViewModel.ResetDefaults()");

            Defaults = new ProductDefaults();
        }

        /// <summary>
        /// Saves defaults as they currently exist.
        /// </summary>
        public MvxCommand SaveDefaultsCommand => new MvxCommand(SaveDefaults);
        void SaveDefaults()
        {
            System.Diagnostics.Debug.WriteLine("ProductDefaultsViewModel.SaveDefaults()");

            ProductDefaults.Write(_defaults.ToJson().ToString());

            Close(this);
        }

        /// <summary>
        /// Compare difference between initial and final.
        /// </summary>
        public async void OnNavigateOut()
        {
            System.Diagnostics.Debug.WriteLine("ProductDefaultsViewModel.OnNavigateOut()");

            if (_dataService.GetDiff(_initialDefaultsJson, _defaults.ToJson().ToString()))
            {
                if (await _dialogService.ShowConfirmMessage(Strings.SaveChangesDialogTitle,
                    Strings.SaveChangesDialogMessage))
                {
                    SaveDefaults();
                }
                else
                {
                    Close(this);
                }
            }
            else
            {
                Close(this);
            }
        }
    }
}
