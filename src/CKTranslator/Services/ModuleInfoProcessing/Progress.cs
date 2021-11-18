using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CKTranslator.Services
{
    public partial class Progress : ObservableObject
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(NormalValue))]
        private int max;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(NormalValue))]
        private int value;

        public double NormalValue => (double) value / max;
    }
}