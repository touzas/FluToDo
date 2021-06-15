using FluToDo.ViewModels;
using FluToDo.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FluToDo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }
    }
}
