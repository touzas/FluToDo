using FluToDo.Models;
using FluToDo.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluToDo.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public const string CMD_TOGGLE = "TOGGLE";
        public const string CMD_ADD = "ADD";
        public const string CMD_EDIT = "EDIT";
        public const string CMD_DELETE = "DELETE";



        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<TodoItem> m_items = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> Items
        {
            get => m_items;
            set
            {
                m_items = value;
                OnPropertyChanged("Items");
            }
        }

        public Command LoadItemsCommand { get; }
        public Command<TodoItem> ToggleCommand { get; }
        public Command<TodoItem> DeleteCommand { get; }
        public Command<TodoItem> EditCommand { get; }
        public Command<TodoItem> AddCommand { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<TodoItem>();
            LoadItemsCommand = new Command(async () => await LoadList());
            ToggleCommand = new Command<TodoItem>(OnToggleComplete);
            DeleteCommand = new Command<TodoItem>(OnDeleteItem);
            EditCommand = new Command<TodoItem>(OnEditItem);
            AddCommand = new Command<TodoItem>(OnAddItem);
        }

        public async void OnToggleComplete(TodoItem item)
        {
            item.IsComplete = !item.IsComplete;
            var res = await DataStore.UpdateItemAsync(item);
            if (res)
            {
                RaisePropertyChanged(CMD_TOGGLE);
                Items = await LoadList();
            }
        }

        public async void OnDeleteItem(TodoItem item)
        {
            var res = await DataStore.DeleteItemAsync(item.Key);
            if (res)
            {
                RaisePropertyChanged(CMD_DELETE);
                Items = await LoadList();
            }
        }
        public async void OnEditItem(TodoItem item)
        {
            var res = await DataStore.UpdateItemAsync(item);
            if (res)
            {
                RaisePropertyChanged(CMD_EDIT);
                Items = await LoadList();
            }
        }
        public async void OnAddItem(TodoItem item)
        {
            var res = await DataStore.CreateItemAsync(item);
            if (res != null)
            {
                RaisePropertyChanged(CMD_ADD);
                Items = await LoadList();
            }
        }
        public async Task<ObservableCollection<TodoItem>> LoadList()
        {
            IsBusy = true;
            try
            {
                Items = await DataStore.GetAsync();
                return Items;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
            return null;
        }
        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}