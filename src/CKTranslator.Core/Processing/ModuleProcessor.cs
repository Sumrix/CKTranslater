using CKTranslator.Core.Models;

using System.Collections.Generic;

namespace CKTranslator.Core.Processing
{
    public class ModuleProcessor
    {
        private readonly ICollection<IModuleProcess> processes;

        private bool isRunning;

        public ModuleProcessor(ICollection<IModuleProcess> processes)
        {
            this.processes = processes;
        }

        public void Process(Module module)
        {
            foreach (var process in processes)
            {
                if (process.IsProcessable(module))
                {
                    foreach (var document in process.SelectDocuments(module))
                    {
                        if (!isRunning)
                        {
                            return;
                        }

                        process.ProcessDocument(document);
                    }
                }
            }
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}