using CKTranslator.Core.Processing;
using CKTranslator.Core.Services;
using CKTranslator.Services;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace CKTranslator.ViewModels
{
    public class ModulesViewModel : ObservableObject
    {
        private readonly ObservableModuleService moduleService;
        private readonly ModuleProcessService processingService;
        private ObservableModuleProcessor? processor;

        public ModulesViewModel(ObservableModuleService moduleService, ModuleProcessService processService)
        {
            Modules = new ObservableCollection<ModuleViewModel>(moduleService.GetModuleViewModels());
            this.moduleService = moduleService;
            this.processingService = processService;
        }

        public ObservableCollection<ModuleViewModel> Modules { get; }

        public void Backup()
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => ProcessModules(processingService.CreateBackupCommand()));
        }

        public void Open()
        {
            var moduleView = Modules.FirstOrDefault(m => m.IsSelected);
            if (moduleView == null)
            {
                return;
            }

            ProcessStartInfo startInfo = new()
            {
                Arguments = moduleView.Module.Path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        public void Recode()
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => ProcessModules(processingService.CreateBackupCommand()));
        }

        public void Restore()
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => ProcessModules(processingService.CreateBackupCommand()));
        }

        public void Stop()
        {
            processor?.Stop();
            moduleService.SaveSettings(Modules);
        }

        public void Translate()
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => ProcessModules(processingService.CreateBackupCommand()));
        }

        private void ProcessModules(IEnumerable<IModuleProcess> processes)
        {
            foreach (var module in Modules)
            {
                module.Progress = 0;
            }

            processor = new ObservableModuleProcessor(processes);
            foreach (var module in Modules)
            {
                if (module.IsSelected)
                {
                    processor.Process(module);
                }
            }

            //moduleService.SaveSettings(Modules);
        }
    }
}