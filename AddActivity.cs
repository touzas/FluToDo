using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluToDo
{
    [Activity(Label = "AddActivity")]
    public class AddActivity : Activity
    {
        private EditText m_etName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_addItem);

            m_etName = FindViewById<EditText>(Resource.Id.et_title);

            Button btAdd = FindViewById<Button>(Resource.Id.btAdd);
            btAdd.Click += AddClick;

            Button btCancel = FindViewById<Button>(Resource.Id.btCancel);
            btCancel.Click += CancelClick;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            this.Finish();
        }

        private async void AddClick(object sender, EventArgs e)
        {            
            var item = new TodoItem();
            item.IsComplete = false;
            item.Name = m_etName.Text;

            var ctr = new TodoController();
            try
            {
                var uri = await ctr.CreateItem(item);
            }
            catch (Exception ex)
            {
                var view = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(view, "Error on create todo", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                Console.WriteLine(ex.Message);
            }
            this.Finish();
        }        
    }
}