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
    [Register("inventory.droid.fragments.EditShoppingItemDialogFrag")]
    public class EditShoppingItemDialogFrag : MvxDialogFragment<EditShoppingItemViewModel>
    {
        /*
        public EditShoppingItemViewModel EditShoppingItemViewModel
        {
            get { return (EditShoppingItemViewModel)ViewModel; }
        }
        */
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            EnsureBindingContextSet(savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.dialog_add_shopping_item, null);

            var dialog = new AlertDialog.Builder(Activity);
            dialog.SetView(view);

            if (ViewModel.IsNew)
            {
                dialog.SetTitle(Strings.AddShoppingItemLabel);
            }
            else
            {
                dialog.SetTitle(Strings.EditShoppingItemLabel);
                dialog.SetNegativeButton(Strings.DeleteLabel, (s, a) => 
                {
                    ViewModel.DeleteShoppingItem();
                });
            }

            dialog.SetPositiveButton(Strings.SaveLabel, (s, a) =>
            {
                ViewModel.SaveShoppingItem();
            });
            return dialog.Create();
        }
    }
}