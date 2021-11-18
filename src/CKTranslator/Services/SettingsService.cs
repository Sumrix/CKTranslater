using System;
using System.Collections.Generic;
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
        private string activePage = string.Empty;
        private ElementTheme appBackgroundRequestedTheme;
        private string gamePath = string.Empty;
        private LocalObjectStorageHelper localStorage;
        private string modsPath = string.Empty;

        public string ActivePage
        {
            get => activePage;
            set => SaveProperty(ref activePage, value);
        }

        public ElementTheme AppBackgroundRequestedTheme
        {
            get => appBackgroundRequestedTheme;
            set => SaveProperty(ref appBackgroundRequestedTheme, value);
        }

        public string GamePath
        {
            get => gamePath;
            set => SaveProperty(ref gamePath, value);
        }

        public string ModsPath
        {
            get => modsPath;
            set => SaveProperty(ref modsPath, value);
        }

        public async Task InitializeAsync()
        {
            localStorage = new LocalObjectStorageHelper(new SystemSerializer());

            appBackgroundRequestedTheme = localStorage.Read<ElementTheme>(nameof(AppBackgroundRequestedTheme));
            gamePath = localStorage.Read<string>(nameof(GamePath));
            modsPath = localStorage.Read<string>(nameof(ModsPath));
            activePage = localStorage.Read<string>(nameof(ActivePage), typeof(GeneralViewModel).FullName);
        }

        public List<string> ReadModuleSettings()
        {
            return localStorage.FileExistsAsync("ModulesSettings").Result
                ? localStorage.ReadFileAsync("ModulesSettings", new List<string>()).Result
                : new List<string>();
        }

        public void SaveModuleSettings(ICollection<string> moduleSettings)
        {
            localStorage.SaveFileAsync("ModuleSettings", moduleSettings).Wait();
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