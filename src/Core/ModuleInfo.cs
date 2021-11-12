using System;
using System.ComponentModel;

namespace Core
{
    /// <summary>
    ///     Информация о моде
    /// </summary>
    public class ModuleInfo : INotifyPropertyChanged
    {
        /// <summary>
        ///     Создан ли бэкап модуля
        /// </summary>
        private bool isBackupped;
        /// <summary>
        ///     Перекодирован ли модуль
        /// </summary>
        private bool isRecoded;
        /// <summary>
        ///     Переведён ли модуль
        /// </summary>
        private bool isTranslated;
        /// <summary>
        ///     Загружён ли перевод
        /// </summary>
        private bool isTranslationLoaded;

        /// <summary>
        ///     Зависимости модуля
        /// </summary>
        public ModuleInfo[] Dependencies { get; set; } = Array.Empty<ModuleInfo>();

        /// <summary>
        ///     Есть ли в моде скрипты
        /// </summary>
        public bool HasScripts { get; set; }

        /// <summary>
        ///     Является ли модуль архивом
        /// </summary>
        public bool IsArchive { get; set; }

        /// <summary>
        ///     Создан ли бэкап модуля
        /// </summary>
        public bool IsBackupped
        {
            get => this.isBackupped;
            set
            {
                this.isBackupped = value;
                this.NotifyPropertyChanged(nameof(this.IsBackupped));
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
        public string Path { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}