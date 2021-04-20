﻿using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace CKTranslator.Views
{
    // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
    // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
    public sealed partial class ModsPage : Page
    {
        public ModsViewModel ViewModel { get; }

        public ModsPage()
        {
            ViewModel = Ioc.Default.GetService<ModsViewModel>();
            InitializeComponent();
        }
    }
}
