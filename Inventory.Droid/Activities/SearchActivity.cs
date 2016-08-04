using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content;
using Inventory.Droid.Adapters;
using Inventory.Model;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;
using Inventory.ViewModels;
using Inventory.Droid.Fragments;
using Android.Views;

namespace Inventory.Droid.Activities
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "inventory.droid.activities.SearchActivity"
        )]
    public class SearchActivity : MvxAppCompatActivity<SearchViewModel>
    {
        protected override void OnViewModelSet()
        {
            System.Diagnostics.Debug.WriteLine("SearchActivity.OnViewModelSet()");

            SetContentView(Resource.Layout.activity_search);

            // Status Bar
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }
    }

        /*
        [Activity(Label = "Search")]
        public class SearchActivity : Activity
        {
            List<Item> items = new List<Item>();

            EditText searchEditText;
            RecyclerView.LayoutManager layoutManager;
            RecyclerView recyclerView;
            ItemAdapter itemAdapter;

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);

                // Layout
                SetContentView(Resource.Layout.Search);

                searchEditText = FindViewById<EditText>(Resource.Id.search_edittext);
                searchEditText.TextChanged += SearchEditText_TextChanged;


                recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerview);
                layoutManager = new LinearLayoutManager(this);
                recyclerView.SetLayoutManager(layoutManager);
            }

            private void SearchEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
            {
                GetResults();
            }

            private async void GetResults()
            {
                /*
                items.Clear();
                items = await Core.Search(searchEditText.Text);

                foreach (Item item in items)
                {
                    System.Diagnostics.Debug.WriteLine(item.Name);
                }

                itemAdapter = new ItemAdapter(this, null, items);


                RunOnUiThread(() => recyclerView.SetAdapter(itemAdapter));
                *//*
            }

            public void ItemSelected(int index)
            {
                ViewItem(items[index].ID);
            }

            private void ViewItem(int id)
            {
                var viewItemActivity = new Intent(this, typeof(ViewItemActivity));

                viewItemActivity.PutExtra("id", id);

                StartActivity(viewItemActivity);
            }
        }
    */
    }