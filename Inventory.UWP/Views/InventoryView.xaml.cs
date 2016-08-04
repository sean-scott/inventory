using Inventory.Model;
using Inventory.ViewModels;
using MvvmCross.Platform;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Inventory.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventoryView
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContext = Mvx.Resolve<InventoryViewModel>();

            //InventoryComboBox.ItemsSource = Mvx.Resolve<MainViewModel>().PresentablePaths;
            //InventoryComboBox.SelectedIndex = 0;
        }

        private void InventoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Item item = e.ClickedItem as Item;

            System.Diagnostics.Debug.WriteLine("Item Name: " + item.Name + "; Item ID: " + item.ID);

            Mvx.Resolve<InventoryViewModel>().ViewItem(item.ID);
        }

        private void InventoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
