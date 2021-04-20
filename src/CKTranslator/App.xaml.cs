using System;

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

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace CKTranslator
{
    public partial class App : Application
    {
        public static Window MainWindow { get; set; }

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs
        }

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
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ILanguageSelectorService, LanguageSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<GeneralViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ModsViewModel>();
            services.AddTransient<ModsPage>();
            //services.AddTransient<SettingsViewModel>();
            //services.AddTransient<SettingsPage>();
            return services.BuildServiceProvider();
        }
    }
}
