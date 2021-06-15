using FluToDo.Models;
using FluToDo.ViewModels;
using FluToDo.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FluToDo.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsViewModel ViewModel { get; private set; }

        public ItemsPage()
        {
            InitializeComponent();
            ViewModel = new ItemsViewModel();
            BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.LoadList();
        }

        protected async void OnRefreshing(object sender, EventArgs e)
        {
            await ViewModel.LoadList();
            todoListView.IsRefreshing = false;
        }
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            var item = mi.CommandParameter as TodoItem;
            if (item == null)
                return;

            ViewModel.DeleteCommand.Execute(item);
            await DisplayAlert("Information", string.Format(CultureInfo.InvariantCulture, "ToDo Item {0} has been deleted correctly", item.Name), "OK");
        }
        public async void OnEdit(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            var item = mi.CommandParameter as TodoItem;
            if (item == null)
                return;

            var addItem = new NewItemPage(item);
            addItem.BindingContext = item;
            await Navigation.PushAsync(addItem);
        }

        protected void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewModel.ToggleCommand.Execute((TodoItem)e.SelectedItem);
            ViewModel.LoadItemsCommand.Execute(null);
        }

        public void OnAddItem(object sender, EventArgs e)
        {
            var todoItem = new TodoItem()
            {
                Key = Guid.NewGuid().ToString()
            };
            var addItem = new NewItemPage();
            addItem.BindingContext = todoItem;
            Navigation.PushAsync(addItem);
        }
    }
}