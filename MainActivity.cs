using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Xamarin.Essentials;

namespace FluToDo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, SwipeRefreshLayout.IOnRefreshListener
    {
        TodoController m_controller;
        SwipeRefreshLayout m_swipeRefreshLayout;
        private ListView m_lvData;
        private List<TodoItem> m_items;
        private TodoAdapter m_adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);            

            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                var view = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(view, "You dont have a internet connection", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            }

            if (CheckSelfPermission(Manifest.Permission.Internet) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.Internet }, 321321321);
            }

            m_controller = new TodoController();
            m_lvData = FindViewById<ListView>(Resource.Id.lvData);            
            m_lvData.ItemClick += TodoClick;
            RegisterForContextMenu(m_lvData);

            m_swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            m_swipeRefreshLayout.SetOnRefreshListener(this);

            GetItems();
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            if (v == m_lvData)
            {
                menu.SetHeaderIcon(Resource.Drawable.fluendo);
                menu.SetHeaderTitle(Resource.String.delete_item);
                menu.Add(0, 0, 0, Resource.String.yes);
                menu.Add(0, 1, 0, Resource.String.no);
            }
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterView.AdapterContextMenuInfo menuInfo = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            switch (item.ItemId)
            {
                case 0:
                    var currentItem = m_items[menuInfo.Position];
                    var res = Task.Run(() =>
                    {
                        return m_controller.DeleteItem(currentItem.Key);
                    });
                    res.Wait();
                    if (res.Result == System.Net.HttpStatusCode.OK)
                    {
                        Toast.MakeText(this, string.Format(CultureInfo.InvariantCulture, GetString(Resource.String.delete_confirm), currentItem.Name), ToastLength.Short).Show();
                        m_adapter.RemoveItem(menuInfo.Position);
                        GetItems();
                    }
                    else
                    {
                        Toast.MakeText(this, string.Format(CultureInfo.InvariantCulture, GetString(Resource.String.delete_cancel), currentItem.Name), ToastLength.Short).Show();
                    }
                    break;
                default:
                    break;
            }
            return base.OnContextItemSelected(item);
        }

        private async void TodoClick(object sender, AdapterView.ItemClickEventArgs e)
        {            
            try
            {
                var currentItem = m_items[e.Position];
                currentItem.IsComplete = !currentItem.IsComplete;
                await m_controller.UpdateItem(currentItem);
                GetItems();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
                drawer.CloseDrawer(GravityCompat.Start);
            else
                base.OnBackPressed();
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            NewActivity(typeof(AddActivity));
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.nav_taskList)
            {
                GetItems();
            }
            else if (id == Resource.Id.nav_taskAdd)
            {
                NewActivity(typeof(AddActivity));
            }            

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        private void NewActivity(Type act)
        {
            var intent = new Android.Content.Intent()
                .AddFlags(Android.Content.ActivityFlags.NewTask)
                .SetClass(Android.App.Application.Context, act);

            Application.Context.StartActivity(intent);
        }
        private async void GetItems()
        {
            m_swipeRefreshLayout.Refreshing = true;
            try
            {                
                m_items = await m_controller.Get();
                m_adapter = new TodoAdapter(this, m_items);
                m_lvData.Adapter = m_adapter;
                m_adapter.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                m_swipeRefreshLayout.Refreshing = false;
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            GetItems();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnRefresh()
        {           
            GetItems();
        }
    }
}

