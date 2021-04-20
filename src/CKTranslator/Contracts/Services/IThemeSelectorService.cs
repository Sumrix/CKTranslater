using System.Threading.Tasks;

using Microsoft.UI.Xaml;

namespace CKTranslator.Contracts.Services
{
    public interface IThemeSelectorService
    {
        Task SetThemeAsync(ElementTheme theme);

        Task SetRequestedThemeAsync();
    }
}
