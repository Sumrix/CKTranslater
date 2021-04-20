using System;
using System.Collections.Generic;
using System.Linq;

using CKTranslator.Contracts.Services;
using CKTranslator.DataAnnotations;
using CKTranslator.Helpers;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace CKTranslator.ViewModels
{
    public class GeneralViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator
    {
        public record LanguageDataModel(string DisplayName, string LanguageTag)
        {
            public override string ToString() => DisplayName;
        }

        public record ThemeDataModel(string Label, ElementTheme Type)
        {
            public override string ToString() => Label.GetLocalized();
        }

        private readonly ISettingsService settings;
        private readonly ILanguageSelectorService languageSelectorService;
        private readonly ICollection<string> settingsMappedProperties = new[]
            {
                nameof(AppBackgroundRequestedTheme),
                nameof(ModsPath),
                nameof(GamePath),
            };

        [DirectoryPath]
        public string GamePath
        {
            get => settings.GamePath;
            set
            {
                settings.GamePath = value;
                ValidateProperty(value);
            }
        }

        [DirectoryPath]
        public string ModsPath
        {
            get => settings.ModsPath;
            set
            {
                settings.ModsPath = value;
                ValidateProperty(value);
            }
        }

        public ICollection<LanguageDataModel> LanguageValues { get; } = (
                from languageTag in ApplicationLanguages.ManifestLanguages
                let language = new Language(languageTag)
                select new LanguageDataModel(language.DisplayName, language.LanguageTag)
            ).ToList();

        private LanguageDataModel language;

        public LanguageDataModel Language
        {
            get => language;
            set => SetProperty(ref language, value, true);
        }

        public IList<ThemeDataModel> ThemeValues { get; } = new ThemeDataModel[]
            {
                new ("DefaultTheme", ElementTheme.Default),
                new ("LightTheme", ElementTheme.Light),
                new ("DarkTheme", ElementTheme.Dark),
            };

        public ThemeDataModel AppBackgroundRequestedTheme
        {
            get => ThemeValues[(int)settings.AppBackgroundRequestedTheme];
            set { if (value != null) settings.AppBackgroundRequestedTheme = value.Type; }
        }

        public string VersionDescription { get; } = GetVersionDescription();

        public GeneralViewModel(ILanguageSelectorService languageSelector, ISettingsService settings)
        {
            this.languageSelectorService = languageSelector;
            this.settings = settings;

            language = LanguageValues.First(l => l.LanguageTag == languageSelector.LanguageTag);

            settings.PropertyChanging += Settings_PropertyChanging;
            settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null && settingsMappedProperties.Contains(e.PropertyName))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }

        private void Settings_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName != null && settingsMappedProperties.Contains(e.PropertyName))
            {
                OnPropertyChanging(e.PropertyName);
            }
        }

        public async void PickModsPath()
        {
            FolderPicker folderPicker = new()
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SettingsIdentifier = "ModsPath",

            };
            folderPicker.Initialize(App.MainWindow);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                //Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("ModsPathToken", folder);
                ModsPath = folder.Path;
            }
        }

        public async void PickGamePath()
        {
            FolderPicker folderPicker = new()
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SettingsIdentifier = "GamePath",
            };
            folderPicker.Initialize(App.MainWindow);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                //StorageApplicationPermissions.FutureAccessList.AddOrReplace("GamePathToken", folder);
                GamePath = folder.Path;
            }
        }

        public async void LanguageChanged()
        {
            if (Language != null)
            {
                await languageSelectorService.SetLanguageAsync(Language.LanguageTag);
            }
        }

        public async void OpenColorsSettings(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        public async void OpenLanguageSettings(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage"));
        }

        private static string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
