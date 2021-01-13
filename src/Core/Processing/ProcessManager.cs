using System.Collections.Generic;

namespace Core.Processing
{
    public static class ProcessManager
    {
        public static IProcess Backup(IList<ModViewData> mods)
        {
            return new Process
            {
                StartStatus = "Резервное копирование файлов...",
                EndStatus = "Резервные копии успешно созданы",
                FileNameGetter = FileManager.GetModFiles,
                FileProcessors = { Backupping.Backup },
                Mods = mods,
                ModProcessedInitializer = (_, e) => e.Mod.ModInfo.IsBackupped = true,
                Condition = mod => !mod.ModInfo.IsBackupped
            };
        }

        public static IEnumerable<Process> LoadTranslation(IList<ModViewData> rusMods,
            IList<ModViewData> engMods,
            Translator translator)
        {
            return new[]
            {
                new Process
                {
                    StartStatus = "Сканирование английских модов...",
                    FileNameGetter = FileManager.GetModFiles,
                    FileProcessors = { translator.LoadEngFiles },
                    Mods = engMods
                },
                new Process
                {
                    StartStatus = "Сканирование русских модов...",
                    EndStatus = "Сканирование завершено",
                    FileNameGetter = FileManager.GetModFiles,
                    FileProcessors = { translator.LoadRusFiles },
                    Mods = rusMods
                }
            };
        }

        public static IProcess Recode(IList<ModViewData> mods)
        {
            return new Process
            {
                StartStatus = "Перекодирование файлов...",
                EndStatus = "Перекодирование завершено",
                FileNameGetter = FileManager.GetModFiles,
                FileProcessors = { FileRecoder.Recode },
                Mods = mods,
                ModProcessedInitializer = (_, e) => e.Mod.ModInfo.IsRecoded = true,
                Condition = mod => !mod.ModInfo.IsRecoded
            };
        }

        public static IProcess Restore(IList<ModViewData> mods)
        {
            return new Process
            {
                StartStatus = "Восстановление файлов...",
                EndStatus = "Файлы успешно восстановлены",
                FileNameGetter = FileManager.GetBackupFiles,
                FileProcessors = { Backupping.Restore },
                Mods = mods,
                ModProcessedInitializer = (_, e) =>
                {
                    e.Mod.ModInfo.IsTranslated = false;
                    e.Mod.ModInfo.IsRecoded = false;
                }
            };
        }

        public static IProcess Translate(IList<ModViewData> mods, Translator translator)
        {
            return new Process
            {
                StartStatus = "Перевод скриптов...",
                EndStatus = "Перевод завершён",
                FileNameGetter = FileManager.GetModFiles,
                FileProcessors = { translator.TranslateScript },
                Mods = mods,
                ModProcessedInitializer = (_, e) => e.Mod.ModInfo.IsTranslated = true
            };
        }
    }
}