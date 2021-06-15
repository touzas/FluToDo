using FluToDo.Models;
using FluToDo.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FluToDo.Views
{
    public partial class NewItemPage : ContentPage
    {
        public ItemsViewModel ViewModel { get; private set; }
        private TodoItem m_currentItem;

        public NewItemPage()
        {
            InitializeComponent();
            ViewModel = new ItemsViewModel();
        }
        public NewItemPage(TodoItem item)
        {
            InitializeComponent();
            ViewModel = new ItemsViewModel();
            m_currentItem = item;
            TaskTitle.Text = item.Name;
        }
        public async void OnCancelClick(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
        public async void OnCreateItem(object sender, EventArgs e)
        {
            var item = new TodoItem()
            {
                Name = TaskTitle.Text,
                IsComplete = false
            };
            ViewModel.AddCommand.Execute(item);
            await Shell.Current.GoToAsync("..");
        }
    }
}