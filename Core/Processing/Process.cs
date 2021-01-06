using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Processing
{
    public class Process : IProcess, INotifyPropertyChanged
    {
        public delegate ICollection<FileContext> GetFileNames(ModInfo mod);

        public delegate Event ProcessFile(FileContext fileName);

        private Dictionary<int, ICollection<FileContext>> modFiles;
        public IList<ModViewData> Mods;
        private Progress progress;
        private string status;

        public Process()
        {
            this.progress = new Progress();
            this.EventLog = new EventLog();
            this.FileProcessors = new List<ProcessFile>();
        }

        public bool CancelRequired { get; private set; }
        public GetFileNames FileNameGetter { get; set; }
        public List<ProcessFile> FileProcessors { get; set; }

        public string Status
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

        public EventHandler<ModEventArgs> ModProcessedInitializer
        {
            set => this.ModProcessed += value;
        }

        public EventLog EventLog { get; }
        public string StartStatus { get; set; }
        public string EndStatus { get; set; }
        public Func<ModViewData, bool> Condition { private get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ModEventArgs> ModProcessed;

        public void Prepare()
        {
            if (!string.IsNullOrEmpty(this.StartStatus))
            {
                this.Status = this.StartStatus;
            }

            this.modFiles = new Dictionary<int, ICollection<FileContext>>();
            int totalFilesAmount = 0;

            for (int i = 0; i < this.Mods.Count; i++)
            {
                ModViewData mod = this.Mods[i];

                if (mod.ModInfo.IsArchive ||
                    !mod.IsChecked ||
                    this.Condition != null && !this.Condition.Invoke(mod))
                {
                    continue;
                }

                var fileNames = this.FileNameGetter(mod.ModInfo);

                if (fileNames != null && fileNames.Count > 0)
                {
                    this.modFiles[i] = fileNames;
                    totalFilesAmount += fileNames.Count;
                }
            }

            this.Progress.Max = totalFilesAmount;
        }

        public void Run()
        {
            foreach (int i in this.modFiles.Keys)
            {
                ModViewData mod = this.Mods[i];
                var files = this.modFiles[i];
                mod.Progress = 0;
                mod.ProgressMax = files.Count;
            }

            foreach (int i in this.modFiles.Keys)
            {
                ModViewData mod = this.Mods[i];
                var files = this.modFiles[i];

                foreach (FileContext file in files)
                {
                    foreach (ProcessFile processFile in this.FileProcessors)
                    {
                        if (this.CancelRequired)
                        {
                            this.Status = "Процесс прерван";
                            return;
                        }

                        Event @event = processFile(file);
                        if (@event != null)
                        {
                            @event.ModName = mod.ModInfo.Name;
                            this.EventLog.Add(@event);
                        }
                    }

                    this.Progress.Value += 1;
                    mod.Progress += 1;
                }

                this.OnModProcessed(mod);
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

        private void OnModProcessed(ModViewData mod)
        {
            this.ModProcessed?.Invoke(this, new ModEventArgs(mod));
        }
    }
}