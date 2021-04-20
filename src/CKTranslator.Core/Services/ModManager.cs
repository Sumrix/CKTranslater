using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Core;
using Core.Processing;
using Core.Storages;

namespace CKTranslator
{
    /// <summary>
    ///     Класс реализующий функционал по работе с модами
    /// </summary>
    public class ModManager : INotifyPropertyChanged
    {
        /// <summary>
        ///     Временный функционал для анализа строк модов
        /// </summary>
        private readonly StringAnalyzer stringsAnalyzer;
        /// <summary>
        ///     Выполняемый процесс обработки модов
        /// </summary>
        private MultiProcess? process;

        public ModManager()
        {
            this.stringsAnalyzer = new StringAnalyzer();
        }

        /// <summary>
        ///     Выполняемый процесс обработки модов
        /// </summary>
        public MultiProcess? Process
        {
            get => this.process;
            set
            {
                this.process = value;
                this.NotifyPropertyChanged(nameof(this.Process));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Провести анализ строк
        /// </summary>
        public void AnalizeStrings()
        {
            this.stringsAnalyzer.Load();
            this.stringsAnalyzer.FilterAndSaveResults();
            //MessageBox.Show("Готово!");
        }

        /// <summary>
        ///     Создать бэкапы модов
        /// </summary>
        public void Backup(IList<ModViewData> mods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(mods)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Представить игру в виде мода
        /// </summary>
        private static ModInfo CreateMainMod(string gamePath)
        {
            ModInfo mainMod = new()
            {
                IsArchive = false,
                Name = "Crusader Kings II",
                Path = gamePath
            };
            int? modFilesCount = FileManager.GetModFiles(mainMod).Count;
            int? backupFilesCount = FileManager.GetBackupFiles(mainMod).Count;
            mainMod.HasScripts = modFilesCount > 0;
            mainMod.IsBackupped = backupFilesCount >= modFilesCount;

            return mainMod;
        }

        /// <summary>
        ///     Загрузить информацию о модах
        /// </summary>
        public static void Load(string modsPath, string gamePath,
            ICollection<string> rusModSettings, ICollection<string> engModSettings,
            IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            var mods = FileManager.LoadMods(modsPath);
            mods.Add(ModManager.CreateMainMod(gamePath));
            InitModlist(rusModSettings, rusMods);
            InitModlist(engModSettings, engMods);

            void InitModlist(ICollection<string> settingLines, ICollection<ModViewData> modViews)
            {
                var modSettings = new Dictionary<string, string>();
                foreach (string line in settingLines)
                {
                    if (line != null)
                    {
                        int pos = line.IndexOf('"');
                        if (pos != -1)
                        {
                            modSettings.Add(line.Substring(0, pos), line[(pos + 1)..]);
                        }
                    }
                }

                foreach (ModInfo mod in mods)
                {
                    ModViewData modView = new(mod);
                    if (modSettings.TryGetValue(mod.Name, out string? settingsLine))
                    {
                        string[] parts = settingsLine.Split('"');
                        modView.IsChecked = ToBool(parts.ElementAtOrDefault(0));
                        modView.ModInfo.IsTranslated = ToBool(parts.ElementAtOrDefault(1));
                        modView.ModInfo.IsRecoded = ToBool(parts.ElementAtOrDefault(2));
                        modView.ModInfo.IsTranslationLoaded = ToBool(parts.ElementAtOrDefault(3));
                    }

                    modViews.Add(modView);
                }

                static bool ToBool(string? value, bool @default = false)
                {
                    return value switch
                    {
                        "1" => true,
                        "0" => false,
                        _ => @default
                    };
                }
            }
        }

        /// <summary>
        ///     Загрузить строки из модов
        /// </summary>
        public void LoadStrings(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    new Process
                    {
                        StartStatus = "Сканирование скриптов игры...",
                        FileNameGetter = FileManager.GetModFiles,
                        FileProcessors = { this.stringsAnalyzer.LoadEngFiles },
                        Mods = engMods
                    },
                    new Process
                    {
                        StartStatus = "Сканирование скриптов русификатора...",
                        EndStatus = "Сканирование завершено",
                        FileNameGetter = FileManager.GetModFiles,
                        FileProcessors = { this.stringsAnalyzer.LoadRusFiles },
                        Mods = ModManager.SortByDependencies(rusMods)
                    }
                }
            };
            this.Process.Run();

            this.stringsAnalyzer.Save();
        }

        /// <summary>
        ///     Загрузить переводы из модов.
        ///     Перед загрузкой будут созданы бэкапы файлов и файлы будут перекодированы.
        /// </summary>
        public void LoadTranslation(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            Translator translator = new();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(engMods),
                    ProcessManager.Recode(engMods),
                    ProcessManager.LoadTranslation(rusMods, engMods, translator)
                }
            };
            this.Process.Run();

            translator.FillDictionary();

            Db.Save();

            foreach (ModViewData mod in rusMods)
            {
                if (!mod.ModInfo.IsArchive && mod.IsChecked)
                {
                    mod.ModInfo.IsTranslationLoaded = true;
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Перекодировать файлы
        /// </summary>
        public void Recode(IList<ModViewData> mods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(mods),
                    ProcessManager.Recode(mods)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Восстановить файлы модов из бэкапа
        /// </summary>
        public void Restore(IList<ModViewData> mods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Restore(mods)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Сохранить информацию о модах
        /// </summary>
        public static (ICollection<string> rusModSettings, ICollection<string> engModSettings) 
            SaveSettings(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            ICollection<string> rusModSettings = new List<string>();
            ICollection<string> engModSettings = new List<string>();
            SaveModlist(rusModSettings, rusMods);
            SaveModlist(engModSettings, engMods);

            static void SaveModlist(ICollection<string> settingLines, IList<ModViewData> modViews)
            {
                settingLines.Clear();
                foreach (ModViewData mod in modViews)
                {
                    string settings = string.Join("\"",
                        mod.ModInfo.Name,
                        ToIntString(mod.IsChecked),
                        ToIntString(mod.ModInfo.IsTranslated),
                        ToIntString(mod.ModInfo.IsRecoded),
                        ToIntString(mod.ModInfo.IsTranslationLoaded)
                    );
                    settingLines.Add(settings);
                }
            }

            return (rusModSettings, engModSettings);

            static string ToIntString(bool value)
            {
                return value ? "1" : "0";
            }
        }

        /// <summary>
        ///     Отсортировать моды в порядке зависимостей
        /// </summary>
        private static IList<ModViewData> SortByDependencies(IList<ModViewData> mods)
        {
            return mods
                .Select(modView => modView.ModInfo)
                .OrderByTopology(modInfo => modInfo.Dependencies, true)
                .Select(modInfo => mods.First(modView => modView.ModInfo == modInfo))
                .ToArray();
        }

        /// <summary>
        ///     Перевести моды
        /// </summary>
        public void Translate(IList<ModViewData> mods)
        {
            Translator translator = new();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(mods),
                    ProcessManager.Recode(mods),
                    ProcessManager.Translate(mods, translator)
                }
            };
            this.Process.Run();

            //var translated = IdManager.Transleated
            //        .ToHashSet()
            //        .OrderBy(x => x);
            //var notTranslated = IdManager.NotTransleated
            //        .ToHashSet()
            //        .OrderBy(x => x)
            //        .ToArray();
            //char[] forbiddenChars = "[]()".ToArray();
            //var toManualTranslate = notTranslated
            //    .Where(x => x.Length < 30);
            //                //x.IndexOfAny(forbiddenChars) < 0);

            //File.WriteAllLines("Transleated.txt", translated);
            //File.WriteAllLines("NotTransleated.txt", notTranslated);
            //File.WriteAllLines("ToManualTranslate.txt", toManualTranslate);
        }
    }
}