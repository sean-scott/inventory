using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;

namespace Inventory.Droid.Fragments
{
    public abstract class BaseFragment : MvxFragment
    {
        //private Toolbar _toolbar;
        //private MvxActionBarDrawerToggle _drawerToggle;

        protected BaseFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(FragmentId, null);

            //_toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            /*
            if (_toolbar != null)
            {
                ((MainActivity)Activity).SetSupportActionBar(_toolbar);
                ((MainActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);

                _drawerToggle = new MvxActionBarDrawerToggle(
                    Activity,                               // host Activity
                    ((MainActivity)Activity).DrawerLayout,  // DrawerLayout object
                    _toolbar,                               // nav drawer icon to replace 'Up' caret
                    Resource.String.drawer_open,            // "open drawer" description
                    Resource.String.drawer_close            // "close drawer" description
                );
                _drawerToggle.DrawerOpened += (object sender, ActionBarDrawerEventArgs e) => ((MainActivity)Activity).HideSoftKeyboard();
                ((MainActivity)Activity).DrawerLayout.SetDrawerListener(_drawerToggle);
            }
            */
            return view;
        }

        protected abstract int FragmentId { get; }
        /*
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (_toolbar != null)
                _drawerToggle.OnConfigurationChanged(newConfig);
        }
        */
        /*
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            if (_toolbar != null)
                _drawerToggle.SyncState();
        }
        */
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : class, IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get { return (TViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }
}