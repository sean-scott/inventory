using Inventory.ViewModels;
using MvvmCross.Platform;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditItemView
    {
        public EditItemView()
        {
            InitializeComponent();
            DataContext = Mvx.Resolve<EditItemViewModel>();
        }

        /// <summary>
        /// Gets the title bar for enabling the back button on PC.
        /// </summary>
        /// <returns>The view of the title bar.</returns>
        private SystemNavigationManager GetTitleBar()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            return currentView;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GetTitleBar().BackRequested += BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            GetTitleBar().BackRequested -= BackRequested;
        }

        private async void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (await Mvx.Resolve<EditItemViewModel>().OnNavigatedOut())
            {
                if (Frame.CanGoBack) Frame.GoBack();
            }
        }

        private void DateTextBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SelectDate();
        }

        private void SelectDate()
        {
            DatePicker datePurchasedPicker = new DatePicker();
            datePurchasedPicker.Header = "Date Purchased";
        }
    }
}
