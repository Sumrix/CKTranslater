using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

namespace CKTranslator.Contracts.Services
{
    /// <summary>
    /// The default <see langword="interface"/> for the settings manager used in the app.
    /// </summary>
    public interface ISettingsService : INotifyPropertyChanged, INotifyPropertyChanging
    {
        ElementTheme AppBackgroundRequestedTheme { get; set; }

        string GamePath { get; set; }

        string ModsPath { get; set; }

        string ActivePage { get; set; }

        List<string> ReadRusModSettings();

        List<string> ReadEngModSettings();

        void SaveRusModSettings(ICollection<string> rusModSettings);

        void SaveEngModSettings(ICollection<string> engModSettings);

        Task InitializeAsync();
    }
}
