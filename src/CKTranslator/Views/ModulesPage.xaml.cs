using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using System;

namespace CKTranslator.Views
{
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
