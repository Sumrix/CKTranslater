using CKTranslator.Activation;
using CKTranslator.Contracts.Services;
using CKTranslator.Contracts.Views;
using CKTranslator.Core.Contracts.Services;
using CKTranslator.Core.Services;
using CKTranslator.Services;
using CKTranslator.ViewModels;
using CKTranslator.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;

using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace CKTranslator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        public static Window MainWindow { get; set; }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService?.ActivateAsync(args);
        }

        private static System.IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            SettingsService settingsService = new();
            services.AddSingleton<ISettingsService, SettingsService>(_ => settingsService);
            services.AddSingleton<IModuleSettingsService, SettingsService>(_ => settingsService);
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ILanguageSelectorService, LanguageSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ObservableModuleService>();
            services.AddSingleton<ModuleProcessService>();
            services.AddSingleton<ModuleService>();

            // Core Services

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<GeneralViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ModulesViewModel>();
            services.AddTransient<ModulesPage>();
            services.AddTransient<DictionaryViewModel>();
            //services.AddTransient<SettingsViewModel>();
            //services.AddTransient<SettingsPage>();
            return services.BuildServiceProvider();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs
        }
    }
}