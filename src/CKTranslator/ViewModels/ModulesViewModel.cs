using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Core;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using CKTranslator.Contracts.Services;
using System.Threading.Tasks;

namespace CKTranslator.ViewModels
{
    public class ModulesViewModel : ObservableObject
    {

        public ModulesViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;

            this.ModManager = new ModulesManager();

            //Settings.Default.RusMods ??= new StringCollection();
            //Settings.Default.EngMods ??= new StringCollection();

            this.RusModules = new ObservableCollection<ModuleViewData>();
            this.EngModules = new ObservableCollection<ModuleViewData>();

            //DependencyPropertyDescriptor dpd =
            //    DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ListView));
            //dpd.AddValueChanged(this.EventsView, this.EventsView_ItemsSourceChanged);

            //this.CheckRusMods.IsSelected = Settings.Default.CheckRusMods;
            //this.CheckEngMods.IsSelected = Settings.Default.CheckEngMods;

            //this.ErrorToggle.IsSelected = Settings.Default.ErrorToggle;
            //this.WarningToggle.IsSelected = Settings.Default.WarningToggle;
            //this.InfoToggle.IsSelected = Settings.Default.InfoToggle;

            //this.DataContext = this;
            //this.RusModsView.Items.SortDescriptions.Add(
            //    new SortDescription("ModInfo.Name", ListSortDirection.Ascending));
            //this.EngModsView.Items.SortDescriptions.Add(
            //    new SortDescription("ModInfo.Name", ListSortDirection.Ascending));

            ModulesManager.Load(settingsService.ModsPath, settingsService.GamePath,
                settingsService.ReadRusModulesSettings(), settingsService.ReadEngModulesSettings(),
                this.RusModules, this.EngModules);

            //this.FilteredRusMods = CollectionViewSource.GetDefaultView(this.RusMods);
            //this.FilteredRusMods.Filter = this.FilterRusMods;

            //this.FilteredEngMods = CollectionViewSource.GetDefaultView(this.EngMods);
            //this.FilteredEngMods.Filter = this.FilterEngMods;

            //ToolTipService.ShowDurationProperty.OverrideMetadata(
            //    typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }

        /// <summary>
        ///     Английские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModuleViewData> EngModules { get; }

        ///// <summary>
        /////     Английские моды отсортированные для вывода на интерфейс
        ///// </summary>
        //private ICollectionView FilteredEngMods { get; }

        ///// <summary>
        /////     События отсортированные для вывода на интерфейс
        ///// </summary>
        //private ICollectionView? FilteredEvents { get; set; }

        ///// <summary>
        /////     Русские моды отсортированные для вывода на интерфейс
        ///// </summary>
        //private ICollectionView FilteredRusMods { get; }

        private readonly ISettingsService settingsService;

        /// <summary>
        ///     Функционал по работе с модами
        /// </summary>
        public ModulesManager ModManager { get; }

        /// <summary>
        ///     Русские моды в порядке загрузки
        /// </summary>
        public ObservableCollection<ModuleViewData> RusModules { get; }

        public void AnalizeStrings_Click()
        {
            this.StartProcess(() => this.ModManager.AnalizeStrings());
        }

        public void Backup()
        {
            this.StartProcess(() => this.ModManager.Backup(this.EngModules));
        }

        //private void CheckEngMods_Click()
        //{
        //    foreach (ModViewData mod in this.EngMods)
        //    {
        //        mod.IsChecked = mod.ModInfo.HasScripts && this.CheckEngMods.IsSelected;
        //    }
        //}

        //private void CheckRusMods_Click()
        //{
        //    foreach (ModViewData mod in this.RusMods)
        //    {
        //        mod.IsChecked = mod.ModInfo.HasScripts && this.CheckRusMods.IsSelected;
        //    }
        //}

        public void Recode()
        {
            this.StartProcess(() => this.ModManager.Recode(this.EngModules));
        }

        //private void EngModOpen_Click()
        //{
        //    ModViewData modView = (ModViewData)this.EngModsView.SelectedItem;
        //    ProcessStartInfo startInfo = new()
        //    {
        //        Arguments = modView.ModInfo.Path,
        //        FileName = "explorer.exe"
        //    };
        //    Process.Start(startInfo);
        //    this.EngModsView.UnselectAll();
        //}

        //private void EngShowAll_Selected()
        //{
        //    this.FilteredEngMods.Refresh();
        //}

        //private void EventsView_ItemsSourceChanged(object? sender, EventArgs e)
        //{
        //    this.FilteredEvents = CollectionViewSource.GetDefaultView(this.EventsView.ItemsSource);
        //    if (this.FilteredEvents != null)
        //    {
        //        this.FilteredEvents.Filter = this.FilterEvents;
        //    }
        //}

        //private void EventsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.EventsView.SelectedItem is Event result)
        //    {
        //        Process.Start(result.FileName);
        //    }
        //}

        //private bool FilterEngMods(object obj)
        //{
        //    if (obj is ModViewData mod)
        //    {
        //        return this.EngShowAll.IsSelected || mod.ModInfo.HasScripts;
        //    }

        //    return false;
        //}

        //private bool FilterEvents(object obj)
        //{
        //    if (obj is Event @event)
        //    {
        //        return this.ErrorToggle.IsSelected && @event.Type == EventType.Error ||
        //               this.WarningToggle.IsSelected && @event.Type == EventType.Warning ||
        //               this.InfoToggle.IsSelected && @event.Type == EventType.Info;
        //    }

        //    return false;
        //}

        //private bool FilterRusMods(object obj)
        //{
        //    if (obj is ModViewData mod)
        //    {
        //        return this.RusShowAll.IsSelected || mod.ModInfo.HasScripts;
        //    }

        //    return false;
        //}

        public void LoadStrings_Click()
        {
            this.StartProcess(() => this.ModManager.LoadStrings(this.RusModules, this.EngModules));
        }

        public void LoadTranslation_Click()
        {
            this.StartProcess(() => this.ModManager.LoadTranslation(this.RusModules, this.EngModules));
        }

        //private void LogFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    this.FilteredEvents?.Refresh();
        //}

        /// <summary>
        ///     Обнулить полосы загрузки
        /// </summary>
        public void ResetProgress()
        {
            foreach (ModuleViewData mod in this.RusModules)
            {
                mod.Progress = 0;
            }

            foreach (ModuleViewData mod in this.EngModules)
            {
                mod.Progress = 0;
            }
        }

        public void Restore()
        {
            this.StartProcess(() => this.ModManager.Restore(this.EngModules));
        }

        //private void RusShowAll_Selected()
        //{
        //    this.FilteredRusMods.Refresh();
        //}

        /// <summary>
        ///     Запустить процесс обработки модов
        /// </summary>
        /// <param name="action"></param>
        private void StartProcess(Action action)
        {
            this.ResetProgress();
            //this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            Task.Run(() =>
            {
                action();
                //this.Dispatcher.Invoke(() =>
                //    this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None
                //);
            });
        }

        public void Stop()
        {
            this.ModManager.Process?.Cancel();
            //this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
        }

        public void Translate()
        {
            this.StartProcess(() => this.ModManager.Translate(this.EngModules));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //if (this.WindowState == WindowState.Maximized)
            //{
            //    // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
            //    Settings.Default.WindowTop = this.RestoreBounds.Top;
            //    Settings.Default.WindowLeft = this.RestoreBounds.Left;
            //    Settings.Default.WindowHeight = this.RestoreBounds.Height;
            //    Settings.Default.WindowWidth = this.RestoreBounds.Width;
            //    Settings.Default.Maximized = true;
            //}
            //else
            //{
            //    Settings.Default.WindowTop = this.Top;
            //    Settings.Default.WindowLeft = this.Left;
            //    Settings.Default.WindowHeight = this.Height;
            //    Settings.Default.WindowWidth = this.Width;
            //    Settings.Default.Maximized = false;
            //}

            //Settings.Default.CheckRusMods = this.CheckRusMods.IsSelected;
            //Settings.Default.CheckEngMods = this.CheckEngMods.IsSelected;

            var (rusModSettings, engModSettings) = 
                ModulesManager.SaveSettings(this.RusModules, this.EngModules);

            settingsService.SaveRusModulesSettings(rusModSettings);
            settingsService.SaveEngModulesSettings(engModSettings);

            //Settings.Default.ErrorToggle = this.ErrorToggle.IsSelected;
            //Settings.Default.WarningToggle = this.WarningToggle.IsSelected;
            //Settings.Default.InfoToggle = this.InfoToggle.IsSelected;

            //Settings.Default.Save();
        }

        public void Open()
        {

        }
    }
}
