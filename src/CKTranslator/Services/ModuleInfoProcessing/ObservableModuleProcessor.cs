using CKTranslator.Core.Processing;
using CKTranslator.ViewModels;

using System.Collections.Generic;
using System.Linq;

namespace CKTranslator.Services
{
    public class ObservableModuleProcessor
    {
        private readonly ICollection<IModuleProcess> processes;

        private bool isRunning;

        public ObservableModuleProcessor(IEnumerable<IModuleProcess> processes)
        {
            this.processes = processes.ToList();
        }

        public void Process(ModuleViewModel moduleView)
        {
            var runningProcesses = processes
                .Where(p => p.IsProcessable(moduleView.Module))
                .ToList();

            moduleView.ProgressMax = runningProcesses.Count * 100;
            moduleView.Progress = 0;
            isRunning = true;

            for (int processIndex = 0; processIndex < runningProcesses.Count; processIndex++)
            {
                var process = runningProcesses[processIndex];
                var documents = process.SelectDocuments(moduleView.Module);

                for (int documentIndex = 0; documentIndex < documents.Count; documentIndex++)
                {
                    if (!isRunning)
                    {
                        return;
                    }

                    process.ProcessDocument(documents[documentIndex]);

                    moduleView.Progress = (int)((processIndex * 100) + (documentIndex / (double)documents.Count * 100));
                }
            }

            moduleView.Progress = moduleView.ProgressMax;
            moduleView.CopyDataFromModel();
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}