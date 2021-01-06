using System.ComponentModel;

namespace Core.Processing
{
    public class Progress : INotifyPropertyChanged
    {
        private int max;
        private int value;

        public int Max
        {
            get => this.max;
            set
            {
                this.max = value;
                this.NotifyPropertyChanged(nameof(this.Max));
            }
        }

        public double NormalValue => (double) this.value / this.max;

        public int Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.NotifyPropertyChanged(nameof(this.Value));
                this.NotifyPropertyChanged(nameof(this.NormalValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}