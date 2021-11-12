using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using System;

namespace CKTranslator.Views
{
    // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
    // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
    public sealed partial class ModulesPage : Page
    {
        public ModulesViewModel ViewModel { get; }

        public ModulesPage()
        {
            ViewModel = Ioc.Default.GetService<ModulesViewModel>() ?? throw new InvalidOperationException("Can't create service"); ;
            InitializeComponent();
        }
    }
}
