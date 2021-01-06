using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;
using CKTranslator.Properties;
using Core;
using Core.Processing;
using Process = System.Diagnostics.Process;

namespace CKTranslator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.ModManager = new ModManager();

            // Инициализация интерфейса
            this.Top = Settings.Default.WindowTop;
            this.Left = Settings.Default.WindowLeft;
            this.Height = Settings.Default.WindowHeight;
            this.Width = Settings.Default.WindowWidth;
            if (Settings.Default.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }

            if (Settings.Default.RusMods == null)
            {
                Settings.Default.RusMods = new StringCollection();
            }

            if (Settings.Default.EngMods == null)
            {
                Settings.Default.EngMods = new StringCollection();
            }

            this.RusMods = new ObservableCollection<ModViewData>();
            this.EngMods = new ObservableCollection<ModViewData>();

            this.InitializeComponent();

            DependencyPropertyDescriptor dpd =
                DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ListView));
            if (dpd != null)
            {
                dpd.AddValueChanged(this.EventsView, this.EventsView_ItemsSourceChanged);
            }

            this.CheckRusMods.IsSelected = Settings.Default.CheckRusMods;
            this.CheckEngMods.IsSelected = Settings.Default.CheckEngMods;

            this.ErrorToggle.IsSelected = Settings.Default.ErrorToggle;
            this.WarningToggle.IsSelected = Settings.Default.WarningToggle;
            this.InfoToggle.IsSelected = Settings.Default.InfoToggle;

            this.DataContext = this;
            this.RusModsView.Items.SortDescriptions.Add(
                new SortDescription("ModInfo.Name", ListSortDirection.Ascending));
            this.EngModsView.Items.SortDescriptions.Add(
                new SortDescription("ModInfo.Name", ListSortDirection.Ascending));

            this.ModManager.Load(this.RusMods, this.EngMods);

            this.FilteredRusMods = CollectionViewSource.GetDefaultView(this.RusMods);
            this.FilteredRusMods.Filter = this.FilterRusMods;

            this.FilteredEngMods = CollectionViewSource.GetDefaultView(this.EngMods);
            this.FilteredEngMods.Filter = this.FilterEngMods;

            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }

        /// <summary>
        ///     Английские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModViewData> EngMods { get; }

        /// <summary>
        ///     Английские моды отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredEngMods { get; }

        /// <summary>
        ///     События отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredEvents { get; private set; }

        /// <summary>
        ///     Русские моды отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredRusMods { get; }

        /// <summary>
        ///     Функционал по работе с модами
        /// </summary>
        public ModManager ModManager { get; set; }

        /// <summary>
        ///     Русские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModViewData> RusMods { get; }

        private void AnalizeStrings_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.AnalizeStrings());
        }

        private void Bakup_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Backup(this.EngMods));
        }

        private void CheckEngMods_Click(object sender, RoutedEventArgs e)
        {
            foreach (ModViewData mod in this.EngMods)
            {
                mod.IsChecked = mod.ModInfo.HasScripts && this.CheckEngMods.IsSelected;
            }
        }

        private void CheckRusMods_Click(object sender, RoutedEventArgs e)
        {
            foreach (ModViewData mod in this.RusMods)
            {
                mod.IsChecked = mod.ModInfo.HasScripts && this.CheckRusMods.IsSelected;
            }
        }

        private void Click_Recode(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Recode(this.EngMods));
        }

        private void EngModOpen_Click(object sender, RoutedEventArgs e)
        {
            ModViewData modView = (ModViewData) this.EngModsView.SelectedItem;
            ProcessStartInfo startInfo = new()
            {
                Arguments = modView.ModInfo.Path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
            this.EngModsView.UnselectAll();
        }

        private void EngShowAll_Selected(object sender, RoutedEventArgs e)
        {
            this.FilteredEngMods?.Refresh();
        }

        private void EventsView_ItemsSourceChanged(object sender, EventArgs e)
        {
            this.FilteredEvents = CollectionViewSource.GetDefaultView(this.EventsView.ItemsSource);
            if (this.FilteredEvents != null)
            {
                this.FilteredEvents.Filter = this.FilterEvents;
            }
        }

        private void EventsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.EventsView.SelectedItem is Event result)
            {
                Process.Start(result.FileName);
            }
        }

        private bool FilterEngMods(object obj)
        {
            if (obj is ModViewData mod)
            {
                return this.EngShowAll.IsSelected || mod.ModInfo.HasScripts;
            }

            return false;
        }

        private bool FilterEvents(object obj)
        {
            if (obj is Event @event)
            {
                return this.ErrorToggle.IsSelected && @event.Type == EventType.Error ||
                       this.WarningToggle.IsSelected && @event.Type == EventType.Warning ||
                       this.InfoToggle.IsSelected && @event.Type == EventType.Info;
            }

            return false;
        }

        private bool FilterRusMods(object obj)
        {
            if (obj is ModViewData mod)
            {
                return this.RusShowAll.IsSelected || mod.ModInfo.HasScripts;
            }

            return false;
        }

        private void LoadStrings_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.LoadStrings(this.RusMods, this.EngMods));
        }

        private void LoadTranslation_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.LoadTranslation(this.RusMods, this.EngMods));
        }

        private void LogFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FilteredEvents?.Refresh();
        }

        /// <summary>
        ///     Обнулить полосы загрузки
        /// </summary>
        private void ResetProgress()
        {
            foreach (ModViewData mod in this.RusMods)
            {
                mod.Progress = 0;
            }

            foreach (ModViewData mod in this.EngMods)
            {
                mod.Progress = 0;
            }
        }

        private void RestoreBakup_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Restore(this.EngMods));
        }

        private void RusShowAll_Selected(object sender, RoutedEventArgs e)
        {
            this.FilteredRusMods?.Refresh();
        }

        /// <summary>
        ///     Запустить процесс обработки модов
        /// </summary>
        /// <param name="action"></param>
        private void StartProcess(Action action)
        {
            this.ResetProgress();
            this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            Task.Run(() =>
            {
                action();
                this.Dispatcher.Invoke(() =>
                    this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None
                );
            });
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.ModManager.Process?.Cancel();
            this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Translate(this.EngMods));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Settings.Default.WindowTop = this.RestoreBounds.Top;
                Settings.Default.WindowLeft = this.RestoreBounds.Left;
                Settings.Default.WindowHeight = this.RestoreBounds.Height;
                Settings.Default.WindowWidth = this.RestoreBounds.Width;
                Settings.Default.Maximized = true;
            }
            else
            {
                Settings.Default.WindowTop = this.Top;
                Settings.Default.WindowLeft = this.Left;
                Settings.Default.WindowHeight = this.Height;
                Settings.Default.WindowWidth = this.Width;
                Settings.Default.Maximized = false;
            }

            Settings.Default.CheckRusMods = this.CheckRusMods.IsSelected;
            Settings.Default.CheckEngMods = this.CheckEngMods.IsSelected;

            ModManager.SaveSettings(this.RusMods, this.EngMods);

            Settings.Default.ErrorToggle = this.ErrorToggle.IsSelected;
            Settings.Default.WarningToggle = this.WarningToggle.IsSelected;
            Settings.Default.InfoToggle = this.InfoToggle.IsSelected;

            Settings.Default.Save();
        }
    }
}