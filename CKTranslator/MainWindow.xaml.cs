using CKTranslator.Processing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;

namespace CKTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Функционал по работе с модами
        /// </summary>
        public ModManager ModManager { get; set; }

        /// <summary>
        /// События отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredEvents { get; private set; }

        /// <summary>
        /// Русские моды отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredRusMods { get; private set; }

        /// <summary>
        /// Английские моды отсортированные для вывода на интерфейс
        /// </summary>
        public ICollectionView FilteredEngMods { get; private set; }

        /// <summary>
        /// Русские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModViewData> RusMods { get; private set; }

        /// <summary>
        /// Английские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModViewData> EngMods { get; private set; }

        public MainWindow()
        {
            this.ModManager = new ModManager();

            // Инициализация интерфейса
            this.Top = Properties.Settings.Default.WindowTop;
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Height = Properties.Settings.Default.WindowHeight;
            this.Width = Properties.Settings.Default.WindowWidth;
            if (Properties.Settings.Default.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }

            if (Properties.Settings.Default.RusMods == null)
            {
                Properties.Settings.Default.RusMods = new System.Collections.Specialized.StringCollection();
            }
            if (Properties.Settings.Default.EngMods == null)
            {
                Properties.Settings.Default.EngMods = new System.Collections.Specialized.StringCollection();
            }

            this.RusMods = new ObservableCollection<ModViewData>();
            this.EngMods = new ObservableCollection<ModViewData>();

            this.InitializeComponent();

            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ListView));
            if (dpd != null)
            {
                dpd.AddValueChanged(this.EventsView, this.EventsView_ItemsSourceChanged);
            }

            this.CheckRusMods.IsSelected = Properties.Settings.Default.CheckRusMods;
            this.CheckEngMods.IsSelected = Properties.Settings.Default.CheckEngMods;

            this.ErrorToggle.IsSelected = Properties.Settings.Default.ErrorToggle;
            this.WarningToggle.IsSelected = Properties.Settings.Default.WarningToggle;
            this.InfoToggle.IsSelected = Properties.Settings.Default.InfoToggle;

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
                typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        private void EventsView_ItemsSourceChanged(object sender, EventArgs e)
        {
            this.FilteredEvents = CollectionViewSource.GetDefaultView(this.EventsView.ItemsSource);
            if (this.FilteredEvents != null)
            {
                this.FilteredEvents.Filter = this.FilterEvents;
            }
        }

        private void Bakup_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Backup(this.EngMods));
        }

        private void RestoreBakup_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Restore(this.EngMods));
        }

        private void LoadTranslation_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.LoadTranslation(this.RusMods, this.EngMods));
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Translate(this.EngMods));
        }

        private void LoadStrings_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.LoadStrings(this.RusMods, this.EngMods));
        }

        private void AnalizeStrings_Click(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.AnalizeStrings());
        }

        private void Click_Recode(object sender, RoutedEventArgs e)
        {
            this.StartProcess(() => this.ModManager.Recode(this.EngMods));
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.ModManager.Process?.Cancel();
            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.WindowTop = this.RestoreBounds.Top;
                Properties.Settings.Default.WindowLeft = this.RestoreBounds.Left;
                Properties.Settings.Default.WindowHeight = this.RestoreBounds.Height;
                Properties.Settings.Default.WindowWidth = this.RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.WindowTop = this.Top;
                Properties.Settings.Default.WindowLeft = this.Left;
                Properties.Settings.Default.WindowHeight = this.Height;
                Properties.Settings.Default.WindowWidth = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.CheckRusMods = this.CheckRusMods.IsSelected;
            Properties.Settings.Default.CheckEngMods = this.CheckEngMods.IsSelected;

            ModManager.SaveSettings(this.RusMods, this.EngMods);

            Properties.Settings.Default.ErrorToggle = this.ErrorToggle.IsSelected;
            Properties.Settings.Default.WarningToggle = this.WarningToggle.IsSelected;
            Properties.Settings.Default.InfoToggle = this.InfoToggle.IsSelected;

            Properties.Settings.Default.Save();
        }

        private bool FilterEvents(object obj)
        {
            if (obj is Event @event)
            {
                return (this.ErrorToggle.IsSelected && @event.Type == EventType.Error) ||
                       (this.WarningToggle.IsSelected && @event.Type == EventType.Warning) ||
                       (this.InfoToggle.IsSelected && @event.Type == EventType.Info);
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

        private bool FilterEngMods(object obj)
        {
            if (obj is ModViewData mod)
            {
                return this.EngShowAll.IsSelected || mod.ModInfo.HasScripts;
            }
            return false;
        }

        private void EventsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.EventsView.SelectedItem is Event result)
            {
                System.Diagnostics.Process.Start(result.FileName);
            }
        }

        private void CheckRusMods_Click(object sender, RoutedEventArgs e)
        {
            foreach (ModViewData mod in this.RusMods)
            {
                mod.IsChecked = mod.ModInfo.HasScripts && this.CheckRusMods.IsSelected;
            }
        }

        private void CheckEngMods_Click(object sender, RoutedEventArgs e)
        {
            foreach (ModViewData mod in this.EngMods)
            {
                mod.IsChecked = mod.ModInfo.HasScripts && this.CheckEngMods.IsSelected;
            }
        }

        private void LogFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FilteredEvents?.Refresh();
        }

        private void RusShowAll_Selected(object sender, RoutedEventArgs e)
        {
            this.FilteredRusMods?.Refresh();
        }

        private void EngShowAll_Selected(object sender, RoutedEventArgs e)
        {
            this.FilteredEngMods?.Refresh();
        }

        private void EngModOpen_Click(object sender, RoutedEventArgs e)
        {
            ModViewData modView = (ModViewData)this.EngModsView.SelectedItem;
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                Arguments = modView.ModInfo.Path,
                FileName = "explorer.exe"
            };
            System.Diagnostics.Process.Start(startInfo);
            this.EngModsView.UnselectAll();
        }

        /// <summary>
        /// Обнулить полосы загрузки
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

        /// <summary>
        /// Запустить процесс обработки модов
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
    }
}
