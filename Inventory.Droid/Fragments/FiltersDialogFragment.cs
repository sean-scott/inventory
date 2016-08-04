using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using Inventory.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using Inventory.Resources;

namespace Inventory.Droid.Fragments
{
    [Register("inventory.droid.fragments.FiltersDialogFragment")]
    public class FiltersDialogFragment : MvxDialogFragment<FiltersViewModel>
    {
        Spinner attributeSpinner;
        Spinner sortSpinner;
        Spinner orderSpinner;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            EnsureBindingContextSet(savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.dialog_filter, null);

            attributeSpinner = view.FindViewById<Spinner>(Resource.Id.dialog_filter_spinner_attribute);
            var attributeAdapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleListItem1, ViewModel.Attributes);
            attributeSpinner.Adapter = attributeAdapter;

            orderSpinner = view.FindViewById<Spinner>(Resource.Id.dialog_filter_spinner_order);
            var orderAdapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleListItem1, ViewModel.Orders);
            orderSpinner.Adapter = orderAdapter;

            sortSpinner = view.FindViewById<Spinner>(Resource.Id.dialog_filter_spinner_sort);
            var sortAdapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleListItem1, ViewModel.Sorts);
            sortSpinner.Adapter = sortAdapter;

            var dialog = new AlertDialog.Builder(Activity);
            dialog.SetView(view);
            dialog.SetPositiveButton(Strings.OkLabel, (s, a) =>
            {
                ViewModel.Filters = new Model.Filters(
                    attributeSpinner.SelectedItemPosition,
                    orderSpinner.SelectedItemPosition, 
                    sortSpinner.SelectedItemPosition);

                ViewModel.SaveFilters();
            });

            SetSpinners();

            return dialog.Create();
        }

        private void SetSpinners()
        {
            attributeSpinner.SetSelection(ViewModel.Filters.Attribute);
            orderSpinner.SetSelection(ViewModel.Filters.Order);
            sortSpinner.SetSelection(ViewModel.Filters.Sort);
        }
    }
}