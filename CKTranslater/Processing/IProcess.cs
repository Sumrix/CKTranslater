using System;
using System.ComponentModel;

namespace CKTranslater.Processing
{
    public class ModEventArgs : EventArgs
    {
        public readonly ModViewData Mod;

        public ModEventArgs(ModViewData mod)
        {
            this.Mod = mod;
        }
    }

    public interface IProcess
    {
        string Status { get; }
        Progress Progress { get; }
        EventLog EventLog { get; }
        string StartStatus { set; }
        string EndStatus { set; }

        event PropertyChangedEventHandler PropertyChanged;
        event EventHandler<ModEventArgs> ModProcessed;
        EventHandler<ModEventArgs> ModProcessedInitializer { set; }
        Func<ModViewData, bool> Condition { set; }

        void Prepare();
        void Run();
        void Cancel();
    }
}
