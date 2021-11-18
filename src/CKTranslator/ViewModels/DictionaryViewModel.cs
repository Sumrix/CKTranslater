using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CKTranslator.ViewModels
{
    public class DictionaryViewModel : ObservableValidator
    {
        public record LoadedDictionary(string Language1Module, string Language2Module);

        public DictionaryViewModel()
        {
            Modules = new string[]
            {
                "Module1",
                "Module2",
                "Module3"
            };
            LoadedDictionaries = new ObservableCollection<LoadedDictionary>
            {
                new("Module1", "Module2")
            };
            LoadDictionaryCommand = new RelayCommand(
                LoadDictionary,
                () => Language1Module is not null && Language2Module is not null);
        }

        private string? language1Module;

        public string? Language1Module
        {
            get => language1Module;
            set
            {
                SetProperty(ref language1Module, value);
                LoadDictionaryCommand.NotifyCanExecuteChanged();
            }
        }

        private string? language2Module;

        public string? Language2Module
        {
            get => language2Module;
            set
            {
                SetProperty(ref language2Module, value);
                LoadDictionaryCommand.NotifyCanExecuteChanged();
            }
        }

        public ICollection<string> Modules { get; }

        public ObservableCollection<LoadedDictionary> LoadedDictionaries { get; }

        public RelayCommand LoadDictionaryCommand { get; }

        public void LoadDictionary()
        {
            LoadedDictionaries.Add(new(Language1Module!, Language2Module!));
        }
    }
}
