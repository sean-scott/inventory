using Inventory.ViewModels;
using MvvmCross.Platform;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Inventory.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewItemView
    {
        public ViewItemView()
        {
            InitializeComponent();
            DataContext = Mvx.Resolve<ViewItemViewModel>();
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

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }
    }
}
