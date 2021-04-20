using System.Threading.Tasks;

using CKTranslator.Contracts.Services;

using Microsoft.UI.Xaml;

namespace CKTranslator.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private readonly ISettingsService settings;

        public ThemeSelectorService(ISettingsService settings)
        {
            this.settings = settings;
            settings.PropertyChanged += SettingsService_PropertyChanged;
        }

        private void SettingsService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISettingsService.AppBackgroundRequestedTheme))
            {
                SetRequestedThemeAsync();
            }
        }

        public async Task SetThemeAsync(ElementTheme theme)
        {
            settings.AppBackgroundRequestedTheme = theme;

            await Task.CompletedTask;
        }

        public async Task SetRequestedThemeAsync()
        {
            if (App.MainWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = settings.AppBackgroundRequestedTheme;
            }

            await Task.CompletedTask;
        }
    }
}
