using System;

using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace CKTranslator.Views
{
    public sealed partial class MainPage : Page
    {
        public GeneralViewModel ViewModel { get; }

        public MainPage()
        {
            ViewModel = Ioc.Default.GetService<GeneralViewModel>() ?? throw new InvalidOperationException("Can't create service");
            InitializeComponent();
        }
    }
}
