using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CKTranslator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DictionaryPage : Page
    {
        public DictionaryViewModel ViewModel { get; }

        public DictionaryPage()
        {
            ViewModel = Ioc.Default.GetService<DictionaryViewModel>() ?? throw new InvalidOperationException("Can't create service"); ;
            this.InitializeComponent();
        }
    }
}
