using CKTranslator.Contracts.Services;
using CKTranslator.Core.Models;
using CKTranslator.Core.Services;
using CKTranslator.Model;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CKTranslator.ViewModels
{
    public class DictionaryViewModel : ObservableValidator
    {
        private readonly ISettingsService settingsService;

        private Module? language1Module;

        private Module? language2Module;

        public DictionaryViewModel(ModuleService moduleService, ISettingsService settingsService)
        {
            this.settingsService = settingsService;

            Modules = moduleService.LoadModules();

            LoadedDictionaries = new ObservableCollection<LoadedDictionary>(settingsService.GetLoadedDictionaries());

            LoadDictionaryCommand = new RelayCommand(
                LoadDictionary,
                () => Language1Module is not null && Language2Module is not null);
        }

        public Module? Language1Module
        {
            get => language1Module;
            set
            {
                SetProperty(ref language1Module, value);
                LoadDictionaryCommand.NotifyCanExecuteChanged();
            }
        }

        public Module? Language2Module
        {
            get => language2Module;
            set
            {
                SetProperty(ref language2Module, value);
                LoadDictionaryCommand.NotifyCanExecuteChanged();
            }
        }

        public RelayCommand LoadDictionaryCommand { get; }

        public ObservableCollection<LoadedDictionary> LoadedDictionaries { get; }

        public ICollection<Module> Modules { get; }

        public void LoadDictionary()
        {
            LoadedDictionaries.Add(new(Language1Module!.Name, Language2Module!.Name));
            // TODO: Make a dictionary loading functionality
            //settingsService.SaveLoadedDictionaries(LoadedDictionaries);
        }
    }
}