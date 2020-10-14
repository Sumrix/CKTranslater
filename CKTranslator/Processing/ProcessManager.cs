using System.Collections.Generic;

namespace CKTranslator.Processing
{
    public static class ProcessManager
    {
        public static IProcess Bakup(IList<ModViewData> mods) => new Process
        {
            StartStatus = "Резервное копирование файлов...",
            EndStatus = "Резервные копии успешно созданы",
            FileNameGetter = FileManager.GetModFiles,
            FileProcessors = { Backuping.Backup },
            Mods = mods,
            ModProcessedInitializer = (o, e) => e.Mod.ModInfo.IsBackuped = true,
            Condition = mod => !mod.ModInfo.IsBackuped
        };

        public static IProcess Restore(IList<ModViewData> mods) => new Process
        {
            StartStatus = "Восстановление файлов...",
            EndStatus = "Файлы успешно восстановлены",
            FileNameGetter = FileManager.GetBackupFiles,
            FileProcessors = { Backuping.Restore },
            Mods = mods,
            ModProcessedInitializer = (o, e) =>
            {
                e.Mod.ModInfo.IsTranslated = false;
                e.Mod.ModInfo.IsRecoded = false;
            }
        };

        public static IProcess Recode(IList<ModViewData> mods) => new Process
        {
            StartStatus = "Перекодирование файлов...",
            EndStatus = "Перекодирование завершено",
            FileNameGetter = FileManager.GetModFiles,
            FileProcessors = { FileRecoder.Recode },
            Mods = mods,
            ModProcessedInitializer = (o, e) => e.Mod.ModInfo.IsRecoded = true,
            Condition = mod => !mod.ModInfo.IsRecoded
        };

        public static IEnumerable<Process> LoadTranslation(IList<ModViewData> rusMods,
                                               IList<ModViewData> engMods,
                                               Translator translator) => new Process[]
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
                StartStatus = "Сканирование русскийх модов...",
                EndStatus = "Сканирование завершено",
                FileNameGetter = FileManager.GetModFiles,
                FileProcessors = { translator.LoadRusFiles },
                Mods = rusMods
            }
        };

        public static IProcess Translate(IList<ModViewData> mods, Translator translator) => new Process
        {
            StartStatus = "Перевод скриптов...",
            EndStatus = "Перевод завершён",
            FileNameGetter = FileManager.GetModFiles,
            FileProcessors = { translator.TranslateScript },
            Mods = mods,
            ModProcessedInitializer = (o, e) => e.Mod.ModInfo.IsTranslated = true
        };
    }
}
