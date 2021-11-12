using System.ComponentModel;

namespace Core
{
    /// <summary>
    ///     Представление мода в интерфейсе
    /// </summary>
    public class ModuleViewData : INotifyPropertyChanged
    {
        private bool isChecked;
        private int progress;
        private int progressMax;

        public ModuleViewData(ModuleInfo moduleInfo)
        {
            this.ModuleInfo = moduleInfo;
            this.progressMax = 100;
            this.progress = 0;
        }

        public bool IsChecked
        {
            get => this.isChecked;
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged(nameof(this.IsChecked));
            }
        }

        public ModuleInfo ModuleInfo { get; }

        public int Progress
        {
            get => this.progress;
            set
            {
                this.progress = value;
                this.NotifyPropertyChanged(nameof(this.Progress));
            }
        }

        public int ProgressMax
        {
            get => this.progressMax;
            set
            {
                this.progressMax = value;
                this.NotifyPropertyChanged(nameof(this.ProgressMax));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}