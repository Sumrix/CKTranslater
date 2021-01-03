using CKTranslator.Processing;
using CKTranslator.Properties;
using CKTranslator.Storages;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace CKTranslator
{
    /// <summary>
    /// Класс реализующий функционал по работе с модами
    /// </summary>
    public class ModManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Временный функционал для анализа строк модов
        /// </summary>
        private readonly StringAnalyzer stringsAnalyzer;

        /// <summary>
        /// Выполняемый процесс обработки модов
        /// </summary>
        private MultiProcess process;
        /// <summary>
        /// Выполняемый процесс обработки модов
        /// </summary>
        public MultiProcess Process
        {
            get => this.process;
            set
            {
                this.process = value;
                this.NotifyPropertyChanged(nameof(this.Process));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ModManager()
        {
            this.stringsAnalyzer = new StringAnalyzer();
        }

        /// <summary>
        /// Представить игру в виде мода
        /// </summary>
        private ModInfo CreateMainMod()
        {
            ModInfo mainMod = new ModInfo
            {
                IsArchive = false,
                Name = "Crusader Kings II",
                Path = Resources.GamePath,
            };
            int? modFilesCount = FileManager.GetModFiles(mainMod)?.Count;
            int? backupFilesCount = FileManager.GetBackupFiles(mainMod)?.Count;
            mainMod.HasScripts = modFilesCount > 0;
            mainMod.IsBackuped = backupFilesCount >= modFilesCount;

            return mainMod;
        }

        /// <summary>
        /// Создать бэкапы модов
        /// </summary>
        public void Backup(IList<ModViewData> mods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Bakup(mods)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        /// Восстановить файлы модов из бекапа
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
        /// Загрузить переводы из модов.
        /// Перед загрузкой будут созданы бэкапы файлов и файлы будут перекодированы.
        /// </summary>
        public void LoadTranslation(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            Translator translator = new Translator();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Bakup(engMods),
                    ProcessManager.Recode(engMods),
                    ProcessManager.LoadTranslation(rusMods, engMods, translator)
                }
            };
            this.Process.Run();

            translator.FillDictionary();

            DB.Save();

            foreach (ModViewData mod in rusMods)
            {
                if (!mod.ModInfo.IsArchive && mod.IsChecked)
                {
                    mod.ModInfo.IsTranslationLoaded = true;
                }
            }
        }

        /// <summary>
        /// Перевести моды
        /// </summary>
        public void Translate(IList<ModViewData> mods)
        {
            Translator translator = new Translator();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Bakup(mods),
                    ProcessManager.Recode(mods),
                    ProcessManager.Translate(mods, translator),
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

        /// <summary>
        /// Загрузить строки из модов
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
                        StartStatus = "Сканирование скиптов игры...",
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
                        Mods = this.SortByDependencies(rusMods)
                    },
                }
            };
            this.Process.Run();

            this.stringsAnalyzer.Save();
        }

        /// <summary>
        /// Провести анализ строк
        /// </summary>
        public void AnalizeStrings()
        {
            this.stringsAnalyzer.Load();
            this.stringsAnalyzer.FilterAndSaveResults();
            MessageBox.Show("Готово!");
        }

        /// <summary>
        /// Перекодировать файлы
        /// </summary>
        public void Recode(IList<ModViewData> mods)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Bakup(mods),
                    ProcessManager.Recode(mods),
                }
            };
            this.Process.Run();
        }

        /// <summary>
        /// Загрузить информацию о модах
        /// </summary>
        public void Load(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            ICollection<ModInfo> mods = FileManager.LoadMods();
            mods.Add(this.CreateMainMod());
            InitModlist(Settings.Default.RusMods, rusMods);
            InitModlist(Settings.Default.EngMods, engMods);

            void InitModlist(StringCollection settingLines, IList<ModViewData> modViews)
            {
                Dictionary<string, string> modSettings = new Dictionary<string, string>();
                foreach (string line in settingLines)
                {
                    int pos = line.IndexOf('"');
                    if (pos != -1)
                    {
                        modSettings.Add(line.Substring(0, pos), line.Substring(pos + 1));
                    }
                }

                foreach (ModInfo mod in mods)
                {
                    ModViewData modView = new ModViewData
                    {
                        ModInfo = mod
                    };
                    if (modSettings.TryGetValue(mod.Name, out string settingsLine))
                    {
                        string[] parts = settingsLine.Split('"');
                        modView.IsChecked = ToBool(parts.ElementAtOrDefault(0));
                        modView.ModInfo.IsTranslated = ToBool(parts.ElementAtOrDefault(1));
                        modView.ModInfo.IsRecoded = ToBool(parts.ElementAtOrDefault(2));
                        modView.ModInfo.IsTranslationLoaded = ToBool(parts.ElementAtOrDefault(3));
                    }
                    modViews.Add(modView);
                }

                static bool ToBool(string value, bool @default = false) => value switch
                {
                    "1" => true,
                    "0" => false,
                    _ => @default
                };
            }
        }

        /// <summary>
        /// Сохранить информацию о модах
        /// </summary>
        public static void SaveSettings(IList<ModViewData> rusMods, IList<ModViewData> engMods)
        {
            SaveModlist(Settings.Default.RusMods, rusMods);
            SaveModlist(Settings.Default.EngMods, engMods);

            static void SaveModlist(StringCollection settingLines, IList<ModViewData> modViews)
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

            static string ToIntString(bool value)
            {
                return value ? "1" : "0";
            }
        }

        /// <summary>
        /// Отсортировать моды в порядке зависимостей
        /// </summary>
        private IList<ModViewData> SortByDependencies(IList<ModViewData> mods)
        {
            Comparer<ModViewData> comparer = Comparer<ModViewData>.Create(
                (a, b) => b.ModInfo.Dependencies != null && b.ModInfo.Dependencies.Contains(a.ModInfo)
                          ? -1
                          : a.ModInfo.Name.CompareTo(b.ModInfo.Name)
            );

            return mods
                .Select(modView => modView.ModInfo)
                .TSort(modInfo => modInfo.Dependencies, true)
                .Select(modInfo => mods.First(modView => modView.ModInfo == modInfo))
                .ToArray();
        }
    }
}
