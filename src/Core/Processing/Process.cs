using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Core.Processing
{
    public class Process : IProcess, INotifyPropertyChanged
    {
        public delegate ICollection<FileContext> GetFileNames(ModuleInfo module);

        public delegate Event? ProcessFile(FileContext fileName);

        private IDictionary<int, ICollection<FileContext>> moduleFiles =
            ImmutableDictionary<int, ICollection<FileContext>>.Empty;
        public IList<ModuleViewData> Modules = ImmutableList<ModuleViewData>.Empty;
        private Progress progress;
        private string? status;

        public Process()
        {
            this.progress = new Progress();
            this.EventLog = new EventLog();
            this.FileProcessors = new List<ProcessFile>();
        }

        public bool CancelRequired { get; private set; }
        public GetFileNames? FileNameGetter { get; set; }
        public List<ProcessFile> FileProcessors { get; set; }

        public string? Status
        {
            get => this.status;
            set
            {
                this.status = value;
                this.NotifyPropertyChanged(nameof(this.Status));
            }
        }

        public Progress Progress
        {
            get => this.progress;
            set
            {
                this.progress = value;
                this.NotifyPropertyChanged(nameof(this.Progress));
            }
        }

        public EventHandler<ModEventArgs> ModuleProcessedInitializer
        {
            set => this.ModuleProcessed += value;
        }

        public EventLog EventLog { get; }
        public string? StartStatus { get; set; }
        public string? EndStatus { get; set; }
        public Func<ModuleViewData, bool>? Condition { private get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<ModEventArgs>? ModuleProcessed;

        public void Prepare()
        {
            if (!string.IsNullOrEmpty(this.StartStatus))
            {
                this.Status = this.StartStatus;
            }

            this.moduleFiles = new Dictionary<int, ICollection<FileContext>>();
            int totalFilesAmount = 0;

            for (int i = 0; i < this.Modules.Count; i++)
            {
                ModuleViewData module = this.Modules[i];

                if (module.ModuleInfo.IsArchive ||
                    !module.IsChecked ||
                    this.Condition != null && !this.Condition(module))
                {
                    continue;
                }

                var fileNames = this.FileNameGetter?.Invoke(module.ModuleInfo);

                if (fileNames != null && fileNames.Count > 0)
                {
                    this.moduleFiles[i] = fileNames;
                    totalFilesAmount += fileNames.Count;
                }
            }

            this.Progress.Max = totalFilesAmount;
        }

        public void Run()
        {
            foreach (int i in this.moduleFiles.Keys)
            {
                ModuleViewData module = this.Modules[i];
                var files = this.moduleFiles[i];
                module.Progress = 0;
                module.ProgressMax = files.Count;
            }

            foreach (int i in this.moduleFiles.Keys)
            {
                ModuleViewData module = this.Modules[i];
                var files = this.moduleFiles[i];

                foreach (FileContext file in files)
                {
                    foreach (ProcessFile processFile in this.FileProcessors)
                    {
                        if (this.CancelRequired)
                        {
                            this.Status = "Процесс прерван";
                            return;
                        }

                        Event? @event = processFile(file);
                        if (@event != null)
                        {
                            @event.ModuleName = module.ModuleInfo.Name;
                            this.EventLog.Add(@event);
                        }
                    }

                    this.Progress.Value += 1;
                    module.Progress += 1;
                }

                this.OnModuleProcessed(module);
            }

            if (!string.IsNullOrEmpty(this.EndStatus))
            {
                this.Status = this.EndStatus;
            }
        }

        public void Cancel()
        {
            this.CancelRequired = true;
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnModuleProcessed(ModuleViewData module)
        {
            this.ModuleProcessed?.Invoke(this, new ModEventArgs(module));
        }
    }
}