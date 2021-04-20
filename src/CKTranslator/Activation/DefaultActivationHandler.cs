using System.Threading.Tasks;

using CKTranslator.Contracts.Services;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;

namespace CKTranslator.Activation
{
    public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly INavigationService navigationService;

        public DefaultActivationHandler(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            string activePage = Ioc.Default.GetRequiredService<ISettingsService>().ActivePage;
            navigationService.NavigateTo(activePage, args.Arguments);
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return navigationService.Frame.Content == null;
        }
    }
}
