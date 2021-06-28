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
        private TodoItem _currentItem;

        public ItemsPage()
        {
            InitializeComponent();
            ViewModel = new ItemsViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            BindingContext = ViewModel;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FluToDo.ViewModels.ItemsViewModel.CMD_DELETE)
                await DisplayAlert("Information", string.Format(CultureInfo.InvariantCulture, "ToDo Item {0} has been deleted correctly", _currentItem.Name), "OK");
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
        public void OnDelete(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            _currentItem = mi.CommandParameter as TodoItem;
            if (_currentItem == null)
                return;

            ViewModel.DeleteCommand.Execute(_currentItem);            
        }
        public async void OnEdit(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            _currentItem = mi.CommandParameter as TodoItem;
            if (_currentItem == null)
                return;

            var addItem = new NewItemPage(_currentItem);
            addItem.BindingContext = _currentItem;
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