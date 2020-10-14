using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CKTranslator.Processing
{
    public class EventLog : INotifyPropertyChanged
    {
        private int errorCount = 0;
        private int warningCount = 0;
        private int infoCount = 0;

        public ObservableCollection<Event> Events { get; set; }

        public int ErrorCount
        {
            get => this.errorCount;
            set
            {
                this.errorCount = value;
                this.NotifyPropertyChanged(nameof(this.ErrorCount));
            }
        }

        public int WarningCount
        {
            get => this.warningCount;
            set
            {
                this.warningCount = value;
                this.NotifyPropertyChanged(nameof(this.WarningCount));
            }
        }

        public int InfoCount
        {
            get => this.infoCount;
            set
            {
                this.infoCount = value;
                this.NotifyPropertyChanged(nameof(this.InfoCount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EventLog()
        {
            this.Events = new ObservableCollection<Event>();
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Add(Event @event)
        {
            switch (@event.Type)
            {
                case EventType.Error: this.ErrorCount++; break;
                case EventType.Warning: this.WarningCount++; break;
                case EventType.Info: this.InfoCount++; break;
            }
            Action action = () => this.Events.Add(@event);
            App.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
