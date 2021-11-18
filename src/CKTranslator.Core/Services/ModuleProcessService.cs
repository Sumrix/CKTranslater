using CKTranslator.Core.Processing;

using System.Collections.Generic;

namespace CKTranslator.Core.Services
{
    public class ModuleProcessService
    {
        public List<IModuleProcess> CreateBackupCommand() => new()
        {
            new BackupProcess(),
        };

        public List<IModuleProcess> CreateRecodeCommand() => new()
        {
            new BackupProcess(),
            new RecodingProcess(),
        };

        public List<IModuleProcess> CreateRestoreCommand() => new()
        {
            new RestoringProcess(),
        };

        public List<IModuleProcess> CreateTranslateCommand() => new()
        {
            new BackupProcess(),
            new RecodingProcess(),
            new TranslatingProcess(),
        };
    }
}