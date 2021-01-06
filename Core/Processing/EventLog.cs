using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Core.Processing
{
    public class EventLog : INotifyPropertyChanged
    {
        private int errorCount;
        private int infoCount;
        private int warningCount;

        public EventLog()
        {
            this.Events = new ObservableCollection<Event>();
        }

        public int ErrorCount
        {
            get => this.errorCount;
            set
            {
                this.errorCount = value;
                this.NotifyPropertyChanged(nameof(this.ErrorCount));
            }
        }

        public ObservableCollection<Event> Events { get; set; }

        public int InfoCount
        {
            get => this.infoCount;
            set
            {
                this.infoCount = value;
                this.NotifyPropertyChanged(nameof(this.InfoCount));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void Add(Event @event)
        {
            switch (@event.Type)
            {
                case EventType.Error:
                    this.ErrorCount++;
                    break;
                case EventType.Warning:
                    this.WarningCount++;
                    break;
                case EventType.Info:
                    this.InfoCount++;
                    break;
            }

            /*Action action = () =>*/
            this.Events.Add(@event);
            //Application.Current.Dispatcher.BeginInvoke(action);
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}