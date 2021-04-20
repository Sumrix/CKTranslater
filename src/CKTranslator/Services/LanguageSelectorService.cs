using System.Linq;
using System.Threading.Tasks;

using CKTranslator.Contracts.Services;

using Microsoft.UI.Xaml;

using Windows.Globalization;

namespace CKTranslator.Services
{
    public class LanguageSelectorService : ILanguageSelectorService
    {
        public string LanguageTag { get; set; } = "";

        public async Task InitializeAsync()
        {
            LanguageTag = ApplicationLanguages.PrimaryLanguageOverride;
            if (this.LanguageTag == "")
            {
                this.LanguageTag =
                    ApplicationLanguages.PrimaryLanguageOverride =
                    ApplicationLanguages.Languages.FirstOrDefault() ??
                    ApplicationLanguages.ManifestLanguages[0];
            }
            await Task.CompletedTask;
        }

        public async Task SetLanguageAsync(string languageTag)
        {
            LanguageTag = languageTag;

            await SetRequestedLanguageAsync();
        }

        public async Task SetRequestedLanguageAsync()
        {
            if (ApplicationLanguages.PrimaryLanguageOverride != this.LanguageTag)
            {
                ApplicationLanguages.PrimaryLanguageOverride = this.LanguageTag;
                //Windows.ApplicationModel.Resources.Core.ResourceContext.ResetGlobalQualifierValues();
                Windows.ApplicationModel.Resources.Core.ResourceContext.SetGlobalQualifierValue("Language", this.LanguageTag);

                if (App.MainWindow.Content is FrameworkElement rootElement)
                {
                    rootElement.Language = this.LanguageTag;
                }
            }

            await Task.CompletedTask;
        }
    }
}
