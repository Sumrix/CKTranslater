using CKTranslator.Contracts.Services;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Navigation;

namespace CKTranslator.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBackEnabled;

        [ObservableProperty]
        private object? selected;

        public INavigationService NavigationService { get; }

        public INavigationViewService NavigationViewService { get; }

        public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
        {
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;
            NavigationViewService = navigationViewService;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            //if (e.SourcePageType == typeof(SettingsPage))
            //{
            //    Selected = NavigationViewService.SettingsItem;
            //    return;
            //}

            var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }
    }
}
