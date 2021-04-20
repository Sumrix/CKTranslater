using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CKTranslator.Activation;
using CKTranslator.Contracts.Services;
using CKTranslator.Contracts.Views;

using Microsoft.UI.Xaml;

namespace CKTranslator.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> defaultHandler;
        private readonly IEnumerable<IActivationHandler> activationHandlers;
        private readonly INavigationService navigationService;
        private readonly IThemeSelectorService themeSelectorService;
        private readonly ILanguageSelectorService languageSelectorService;
        private readonly ISettingsService settingsService;

        public ActivationService(IShellWindow shellWindow, ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
            IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService,
            IThemeSelectorService themeSelectorService, ILanguageSelectorService languageSelectorService,
            ISettingsService settingsService)
        {
            App.MainWindow = shellWindow as Window;

            this.defaultHandler = defaultHandler;
            this.activationHandlers = activationHandlers;
            this.navigationService = navigationService;
            this.themeSelectorService = themeSelectorService;
            this.languageSelectorService = languageSelectorService;
            this.settingsService = settingsService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            // Initialize services that you need before app activation
            // take into account that the splash screen is shown while this code runs.
            await InitializeAsync();

            App.MainWindow.Activate();

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Tasks after activation
            await StartupAsync();
        }

        public async void Activate(object activationArgs)
        {
            // Initialize services that you need before app activation
            // take into account that the splash screen is shown while this code runs.
            await InitializeAsync();

            App.MainWindow.Activate();

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Tasks after activation
            await StartupAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = activationHandlers
                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (defaultHandler.CanHandle(activationArgs))
            {
                await defaultHandler.HandleAsync(activationArgs);
            }
        }

        private async Task InitializeAsync()
        {
            await settingsService.InitializeAsync().ConfigureAwait(false);
            await languageSelectorService.InitializeAsync().ConfigureAwait(false);
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            await themeSelectorService.SetRequestedThemeAsync();
            await languageSelectorService.SetRequestedLanguageAsync();
            await Task.CompletedTask;
        }
    }
}
