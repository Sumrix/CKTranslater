using System.ComponentModel;
using System.Threading.Tasks;

using CKTranslator.Core.Contracts.Services;

using Microsoft.UI.Xaml;

namespace CKTranslator.Contracts.Services
{
    /// <summary>
    /// The default <see langword="interface"/> for the settings manager used in the app.
    /// </summary>
    public interface ISettingsService : IModuleSettingsService, INotifyPropertyChanged, INotifyPropertyChanging
    {
        string ActivePage { get; set; }

        ElementTheme AppBackgroundRequestedTheme { get; set; }

        Task InitializeAsync();
    }
}