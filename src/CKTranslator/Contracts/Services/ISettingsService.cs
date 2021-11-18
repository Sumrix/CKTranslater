using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using CKTranslator.Core.Contracts.Services;
using CKTranslator.Model;

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

        IEnumerable<LoadedDictionary> GetLoadedDictionaries();

        Task InitializeAsync();

        void SaveLoadedDictionaries(ICollection<LoadedDictionary> loadedDictionaries);
    }
}