using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace CKTranslater.Processing
{
    public class MultiProcess : IEnumerable<IProcess>, INotifyPropertyChanged
    {
        private IProcess runningProcess;
        private string status;
        private Progress progress;

        public bool CancelRequired { get; private set; }

        public string SubStatus => this.runningProcess?.Status ?? this.EndStatus;
        public Progress SubProgress => this.runningProcess?.Progress;
        public EventLog SubEventLog =>this.runningProcess?.EventLog;
        public event PropertyChangedEventHandler PropertyChanged;
        public string StartStatus { get; set; } = "Общий прогресс...";
        public string EndStatus { get; set; } = "Общий прогресс окончен";

        public List<IProcess> Processes { get; }
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

        public MultiProcess()
        {
            this.Processes = new List<IProcess>();
            this.progress = new Progress();
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Add(IProcess process)
        {
            this.Processes.Add(process);
        }

        public void Add(IEnumerable<Process> processes)
        {
            foreach (Process process in processes)
            {
                this.Add(process);
            }
        }

        public void Run()
        {
            this.Status = this.StartStatus;
            this.Progress.Max = 0;
            this.Progress.Value = 0;

            foreach (IProcess process in this.Processes)
            {
                process.Prepare();
                this.Progress.Max += process.Progress.Max;
            }

            foreach (IProcess process in this.Processes)
            {
                if (this.CancelRequired)
                {
                    return;
                }

                this.runningProcess = process;
                this.NotifyPropertyChanged(nameof(this.SubStatus));
                this.NotifyPropertyChanged(nameof(this.SubProgress));
                this.NotifyPropertyChanged(nameof(this.SubEventLog));
                process.PropertyChanged += this.Process_PropertyChanged;
                process.Progress.PropertyChanged += this.Progress_PropertyChanged;
                process.Run();
                process.Progress.PropertyChanged -= this.Progress_PropertyChanged;
                process.PropertyChanged -= this.Process_PropertyChanged;
            }

            this.Status = this.EndStatus;
        }

        private void Progress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                this.Progress.Value++;
            }
        }

        private void Process_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(e.PropertyName);
        }

        public void Cancel()
        {
            this.CancelRequired = true;
            this.runningProcess?.Cancel();
        }

        public IEnumerator<IProcess> GetEnumerator()
        {
            return this.Processes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
