using System;
using System.ComponentModel;

namespace Core.Processing
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
        Func<ModViewData, bool> Condition { set; }
        string EndStatus { set; }
        EventLog EventLog { get; }
        EventHandler<ModEventArgs> ModProcessedInitializer { set; }
        Progress Progress { get; }
        string StartStatus { set; }
        string? Status { get; }
        void Cancel();
        event EventHandler<ModEventArgs> ModProcessed;
        void Prepare();
        event PropertyChangedEventHandler PropertyChanged;
        void Run();
    }
}