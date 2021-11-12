using System;
using System.ComponentModel;

namespace Core.Processing
{
    public class ModEventArgs : EventArgs
    {
        public readonly ModuleViewData Module;

        public ModEventArgs(ModuleViewData mod)
        {
            this.Module = mod;
        }
    }

    public interface IProcess
    {
        Func<ModuleViewData, bool> Condition { set; }
        string EndStatus { set; }
        EventLog EventLog { get; }
        EventHandler<ModEventArgs> ModuleProcessedInitializer { set; }
        Progress Progress { get; }
        string StartStatus { set; }
        string? Status { get; }
        void Cancel();
        event EventHandler<ModEventArgs> ModuleProcessed;
        void Prepare();
        event PropertyChangedEventHandler PropertyChanged;
        void Run();
    }
}