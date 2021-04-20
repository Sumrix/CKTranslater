using System.Threading.Tasks;

namespace CKTranslator.Contracts.Services
{
    public interface ILanguageSelectorService
    {
        string LanguageTag { get; }

        Task InitializeAsync();

        Task SetLanguageAsync(string languageTag);

        Task SetRequestedLanguageAsync();
    }
}
