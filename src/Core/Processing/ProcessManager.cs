using System.Collections.Generic;

namespace Core.Processing
{
    public static class ProcessManager
    {
        public static IProcess Backup(IList<ModuleViewData> modules)
        {
            return new Process
            {
                StartStatus = "Резервное копирование файлов...",
                EndStatus = "Резервные копии успешно созданы",
                FileNameGetter = FileManager.GetModuleFiles,
                FileProcessors = { Backupping.Backup },
                Modules = modules,
                ModuleProcessedInitializer = (_, e) => e.Module.ModuleInfo.IsBackupped = true,
                Condition = mod => !mod.ModuleInfo.IsBackupped
            };
        }

        public static IEnumerable<Process> LoadTranslation(
            IList<ModuleViewData> rusModules,
            IList<ModuleViewData> engModules,
            Translator translator)
        {
            return new[]
            {
                new Process
                {
                    StartStatus = "Сканирование английских модов...",
                    FileNameGetter = FileManager.GetModuleFiles,
                    FileProcessors = { translator.LoadEngFiles },
                    Modules = engModules
                },
                new Process
                {
                    StartStatus = "Сканирование русских модов...",
                    EndStatus = "Сканирование завершено",
                    FileNameGetter = FileManager.GetModuleFiles,
                    FileProcessors = { translator.LoadRusFiles },
                    Modules = rusModules
                }
            };
        }

        public static IProcess Recode(IList<ModuleViewData> modules)
        {
            return new Process
            {
                StartStatus = "Перекодирование файлов...",
                EndStatus = "Перекодирование завершено",
                FileNameGetter = FileManager.GetModuleFiles,
                FileProcessors = { FileRecoder.Recode },
                Modules = modules,
                ModuleProcessedInitializer = (_, e) => e.Module.ModuleInfo.IsRecoded = true,
                Condition = mod => !mod.ModuleInfo.IsRecoded
            };
        }

        public static IProcess Restore(IList<ModuleViewData> modules)
        {
            return new Process
            {
                StartStatus = "Восстановление файлов...",
                EndStatus = "Файлы успешно восстановлены",
                FileNameGetter = FileManager.GetBackupFiles,
                FileProcessors = { Backupping.Restore },
                Modules = modules,
                ModuleProcessedInitializer = (_, e) =>
                {
                    e.Module.ModuleInfo.IsTranslated = false;
                    e.Module.ModuleInfo.IsRecoded = false;
                }
            };
        }

        public static IProcess Translate(IList<ModuleViewData> modules, Translator translator)
        {
            return new Process
            {
                StartStatus = "Перевод скриптов...",
                EndStatus = "Перевод завершён",
                FileNameGetter = FileManager.GetModuleFiles,
                FileProcessors = { translator.TranslateScript },
                Modules = modules,
                ModuleProcessedInitializer = (_, e) => e.Module.ModuleInfo.IsTranslated = true
            };
        }
    }
}