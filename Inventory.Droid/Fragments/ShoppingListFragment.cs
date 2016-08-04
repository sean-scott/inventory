using MvvmCross.Droid.Support.V4;
using Inventory.ViewModels;
using Android.Runtime;
using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using MvvmCross.Platform.WeakSubscription;
using MvvmCross.Core.ViewModels;
using System.ComponentModel;
using Android.Widget;

namespace Inventory.Droid.Fragments
{
    [Register("inventory.droid.fragments.ShoppingListFragment")]
    public class ShoppingListFragment : MvxFragment<ShoppingListViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_shopping_list, null);

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.fragment_shopping_list_recycler_view);
            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;
                var layoutManager = new LinearLayoutManager(Activity);
                recyclerView.SetLayoutManager(layoutManager);
            }

            var swipeRefreshLayout = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.fragment_shopping_list_refresher);

            var createShoppingItemButton = view.FindViewById<Button>(Resource.Id.fragment_shopping_list_button_create);
            createShoppingItemButton.Click += CreateShoppingItemButton_Click;

            var createShoppingItemFab = view.FindViewById<FloatingActionButton>(Resource.Id.fragment_shopping_list_fab_create);
            createShoppingItemFab.Click += CreateShoppingItemFab_Click;

            // listen for item selection
            IMvxNotifyPropertyChanged itemSelected = ViewModel as IMvxNotifyPropertyChanged;
            itemSelected.WeakSubscribe(OnItemSelected);

            return view;
        }

        private void CreateShoppingItemButton_Click(object sender, System.EventArgs e)
        {
            DisplayDialog(0);
        }

        private void CreateShoppingItemFab_Click(object sender, System.EventArgs e)
        {
            DisplayDialog(0);
        }

        private void OnItemSelected(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ShoppingListFragment.OnItemSelected");

            if (e.PropertyName == "SelectedId")
            {
                System.Diagnostics.Debug.WriteLine("SelectedId: " + ViewModel.SelectedId + ", ItemId: " + ViewModel.SelectedItemId);

                // only display dialog for existing shopping items
                // without corresponding inventory item.
                if (ViewModel.SelectedId > 0)
                {
                    DisplayDialog(ViewModel.SelectedId);
                }
            }
        }

        /// <summary>
        /// Displays dialog for creating or editing a shopping item.
        /// </summary>
        /// <param name="id">
        /// The ID of the shopping item to be edited.
        /// If the ID is 0, the dialog assumes new item.
        /// </param>
        private void DisplayDialog(int id)
        {
            var dialog = new EditShoppingItemDialogFrag();
            dialog.ViewModel = new EditShoppingItemViewModel(ViewModel.DataService);
            dialog.ViewModel.Init(id);
            dialog.Show(FragmentManager, "");
            ViewModel.SelectedId = 0; // reset in case of item re-selection (#MVVMProblems)
        }
    }
}