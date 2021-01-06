using System.ComponentModel;

namespace Core
{
    /// <summary>
    ///     Информация о моде
    /// </summary>
    public class ModInfo : INotifyPropertyChanged
    {
        /// <summary>
        ///     Создан ли бэкап мода
        /// </summary>
        private bool isBackuped;
        /// <summary>
        ///     Перекодирован ли мод
        /// </summary>
        private bool isRecoded;
        /// <summary>
        ///     Переведён ли мод
        /// </summary>
        private bool isTranslated;
        /// <summary>
        ///     Загружён ли перевод
        /// </summary>
        private bool isTranslationLoaded;

        /// <summary>
        ///     Зависимости мода
        /// </summary>
        public ModInfo[] Dependencies { get; set; }

        /// <summary>
        ///     Есть ли в моде скрипты
        /// </summary>
        public bool HasScripts { get; set; }

        /// <summary>
        ///     Является ли мод архивом
        /// </summary>
        public bool IsArchive { get; set; }

        /// <summary>
        ///     Создан ли бэкап мода
        /// </summary>
        public bool IsBackuped
        {
            get => this.isBackuped;
            set
            {
                this.isBackuped = value;
                this.NotifyPropertyChanged(nameof(this.IsBackuped));
            }
        }

        /// <summary>
        ///     Перекодирован ли мод
        /// </summary>
        public bool IsRecoded
        {
            get => this.isRecoded;
            set
            {
                this.isRecoded = value;
                this.NotifyPropertyChanged(nameof(this.IsRecoded));
            }
        }

        /// <summary>
        ///     Переведён ли мод
        /// </summary>
        public bool IsTranslated
        {
            get => this.isTranslated;
            set
            {
                this.isTranslated = value;
                this.NotifyPropertyChanged(nameof(this.IsTranslated));
            }
        }

        /// <summary>
        ///     Загружён ли перевод
        /// </summary>
        public bool IsTranslationLoaded
        {
            get => this.isTranslationLoaded;
            set
            {
                this.isTranslationLoaded = value;
                this.NotifyPropertyChanged(nameof(this.IsTranslationLoaded));
            }
        }

        /// <summary>
        ///     Наименование мода
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Путь к файлам мода
        /// </summary>
        public string Path { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}