using System.ComponentModel;

namespace CKTranslater.Processing
{
    public class Progress : INotifyPropertyChanged
    {
        private int value;
        private int max;

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

        public double NormalValue
        {
            get => (double)this.value / this.max;
        }

        public int Max
        {
            get => this.max;
            set
            {
                this.max = value;
                this.NotifyPropertyChanged(nameof(this.Max));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
