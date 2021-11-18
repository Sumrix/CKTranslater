using CKTranslator.Core.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CKTranslator.ViewModels
{
    public partial class ModuleViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBackuped;

        [ObservableProperty]
        private bool isRecoded;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private bool isTranslated;

        [ObservableProperty]
        private int progress;

        [ObservableProperty]
        private int progressMax = 100;

        public ModuleViewModel(Module module)
        {
            Module = module;
            CopyDataFromModel();
        }

        public Module Module { get; set; }

        public void CopyDataFromModel()
        {
            IsBackuped = Module.IsBackuped;
            IsRecoded = Module.IsRecoded;
            IsTranslated = Module.IsTranslated;
        }
    }
}