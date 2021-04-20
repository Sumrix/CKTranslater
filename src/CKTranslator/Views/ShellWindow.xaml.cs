using CKTranslator.Contracts.Views;
using CKTranslator.Helpers;
using CKTranslator.ViewModels;

using Microsoft.UI.Xaml;

namespace CKTranslator.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellWindow.xaml.
    public sealed partial class ShellWindow : Window, IShellWindow
    {
        public ShellViewModel ViewModel { get; }

        public ShellWindow(ShellViewModel viewModel)
        {
            Title = "AppDisplayName".GetLocalized();
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
        }
    }
}
