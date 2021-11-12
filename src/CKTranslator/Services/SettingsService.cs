using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using CKTranslator.Contracts.Services;
using CKTranslator.ViewModels;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.UI.Xaml;

namespace CKTranslator.Services
{
    public sealed class SettingsService : ObservableObject, ISettingsService
    {
        private LocalObjectStorageHelper localStorage;

        private ElementTheme appBackgroundRequestedTheme;
        public ElementTheme AppBackgroundRequestedTheme
        {
            get => appBackgroundRequestedTheme;
            set => SaveProperty(ref appBackgroundRequestedTheme, value);
        }

        private string gamePath = string.Empty;
        public string GamePath
        {
            get => gamePath;
            set => SaveProperty(ref gamePath, value);
        }

        private string modsPath = string.Empty;
        public string ModsPath
        {
            get => modsPath;
            set => SaveProperty(ref modsPath, value);
        }

        private string activePage = string.Empty;
        public string ActivePage
        {
            get => activePage;
            set => SaveProperty(ref activePage, value);
        }

        public List<string> ReadRusModulesSettings()
        {
            return localStorage.FileExistsAsync("RusModulesSettings").Result
                ? localStorage.ReadFileAsync("RusModulesSettings", new List<string>()).Result
                : new List<string>();
        }

        public List<string> ReadEngModulesSettings()
        {
            return localStorage.FileExistsAsync("EngModulesSettings").Result
                ? localStorage.ReadFileAsync("EngModulesSettings", new List<string>()).Result
                : new List<string>();
        }

        public void SaveRusModulesSettings(ICollection<string> rusModulesSettings)
        {
            localStorage.SaveFileAsync("RusModulesSettings", rusModulesSettings).Wait();
        }

        public void SaveEngModulesSettings(ICollection<string> engModulesSettings)
        {
            localStorage.SaveFileAsync("EngModulesSettings", engModulesSettings).Wait();
        }

        public async Task InitializeAsync()
        {
            localStorage = new LocalObjectStorageHelper(new SystemSerializer());

            appBackgroundRequestedTheme = localStorage.Read<ElementTheme>(nameof(AppBackgroundRequestedTheme));
            gamePath = localStorage.Read<string>(nameof(GamePath));
            modsPath = localStorage.Read<string>(nameof(ModsPath));
            activePage = localStorage.Read<string>(nameof(ActivePage), typeof(GeneralViewModel).FullName);
        }

        private bool SaveProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            localStorage.Save(propertyName, newValue);
            field = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
