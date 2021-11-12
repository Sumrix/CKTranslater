using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Core;
using Core.Processing;
using Core.Storages;

namespace CKTranslator
{
    /// <summary>
    ///     Класс реализующий функционал по работе с модулями
    /// </summary>
    public class ModulesManager : INotifyPropertyChanged
    {
        /// <summary>
        ///     Временный функционал для анализа строк модулей
        /// </summary>
        private readonly StringAnalyzer stringsAnalyzer;
        /// <summary>
        ///     Выполняемый процесс обработки модулей
        /// </summary>
        private MultiProcess? process;

        public ModulesManager()
        {
            this.stringsAnalyzer = new StringAnalyzer();
        }

        /// <summary>
        ///     Выполняемый процесс обработки модулей
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
        ///     Создать бэкапы модулей
        /// </summary>
        public void Backup(IList<ModuleViewData> modules)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(modules)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Представить игру в виде модуля
        /// </summary>
        private static ModuleInfo CreateMainModule(string gamePath)
        {
            ModuleInfo mainModule = new()
            {
                IsArchive = false,
                Name = "Crusader Kings II",
                Path = gamePath
            };
            int? modFilesCount = FileManager.GetModuleFiles(mainModule).Count;
            int? backupFilesCount = FileManager.GetBackupFiles(mainModule).Count;
            mainModule.HasScripts = modFilesCount > 0;
            mainModule.IsBackupped = backupFilesCount >= modFilesCount;

            return mainModule;
        }

        /// <summary>
        ///     Загрузить информацию о модулях
        /// </summary>
        public static void Load(string modsPath, string gamePath,
            ICollection<string> rusModulesSettings, ICollection<string> engModulesSettings,
            IList<ModuleViewData> rusModules, IList<ModuleViewData> engModules)
        {
            var modules = FileManager.LoadModules(modsPath);
            modules.Add(ModulesManager.CreateMainModule(gamePath));
            InitModlist(rusModulesSettings, rusModules);
            InitModlist(engModulesSettings, engModules);

            void InitModlist(ICollection<string> settingLines, ICollection<ModuleViewData> modViews)
            {
                var modulesSettings = new Dictionary<string, string>();
                foreach (string line in settingLines)
                {
                    if (line != null)
                    {
                        int pos = line.IndexOf('"');
                        if (pos != -1)
                        {
                            modulesSettings.Add(line.Substring(0, pos), line[(pos + 1)..]);
                        }
                    }
                }

                foreach (ModuleInfo module in modules)
                {
                    ModuleViewData modView = new(module);
                    if (modulesSettings.TryGetValue(module.Name, out string? settingsLine))
                    {
                        string[] parts = settingsLine.Split('"');
                        modView.IsChecked = ToBool(parts.ElementAtOrDefault(0));
                        modView.ModuleInfo.IsTranslated = ToBool(parts.ElementAtOrDefault(1));
                        modView.ModuleInfo.IsRecoded = ToBool(parts.ElementAtOrDefault(2));
                        modView.ModuleInfo.IsTranslationLoaded = ToBool(parts.ElementAtOrDefault(3));
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
        ///     Загрузить строки из модулей
        /// </summary>
        public void LoadStrings(IList<ModuleViewData> rusModules, IList<ModuleViewData> engModules)
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
                        FileNameGetter = FileManager.GetModuleFiles,
                        FileProcessors = { this.stringsAnalyzer.LoadEngFiles },
                        Modules = engModules
                    },
                    new Process
                    {
                        StartStatus = "Сканирование скриптов русификатора...",
                        EndStatus = "Сканирование завершено",
                        FileNameGetter = FileManager.GetModuleFiles,
                        FileProcessors = { this.stringsAnalyzer.LoadRusFiles },
                        Modules = ModulesManager.SortByDependencies(rusModules)
                    }
                }
            };
            this.Process.Run();

            this.stringsAnalyzer.Save();
        }

        /// <summary>
        ///     Загрузить переводы из модулей.
        ///     Перед загрузкой будут созданы бэкапы файлов и файлы будут перекодированы.
        /// </summary>
        public void LoadTranslation(IList<ModuleViewData> rusModules, IList<ModuleViewData> engModules)
        {
            Translator translator = new();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(engModules),
                    ProcessManager.Recode(engModules),
                    ProcessManager.LoadTranslation(rusModules, engModules, translator)
                }
            };
            this.Process.Run();

            translator.FillDictionary();

            Db.Save();

            foreach (ModuleViewData module in rusModules)
            {
                if (!module.ModuleInfo.IsArchive && module.IsChecked)
                {
                    module.ModuleInfo.IsTranslationLoaded = true;
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
        public void Recode(IList<ModuleViewData> modules)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(modules),
                    ProcessManager.Recode(modules)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Восстановить файлы модулей из бэкапа
        /// </summary>
        public void Restore(IList<ModuleViewData> modules)
        {
            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Restore(modules)
                }
            };
            this.Process.Run();
        }

        /// <summary>
        ///     Сохранить информацию о модулях
        /// </summary>
        public static (ICollection<string> rusModulesSettings, ICollection<string> engModulesSettings) 
            SaveSettings(IList<ModuleViewData> rusModules, IList<ModuleViewData> engModules)
        {
            ICollection<string> rusModulesSettings = new List<string>();
            ICollection<string> engModulesSettings = new List<string>();
            SaveModlist(rusModulesSettings, rusModules);
            SaveModlist(engModulesSettings, engModules);

            static void SaveModlist(ICollection<string> settingLines, IList<ModuleViewData> modViews)
            {
                settingLines.Clear();
                foreach (ModuleViewData mod in modViews)
                {
                    string settings = string.Join("\"",
                        mod.ModuleInfo.Name,
                        ToIntString(mod.IsChecked),
                        ToIntString(mod.ModuleInfo.IsTranslated),
                        ToIntString(mod.ModuleInfo.IsRecoded),
                        ToIntString(mod.ModuleInfo.IsTranslationLoaded)
                    );
                    settingLines.Add(settings);
                }
            }

            return (rusModulesSettings, engModulesSettings);

            static string ToIntString(bool value)
            {
                return value ? "1" : "0";
            }
        }

        /// <summary>
        ///     Отсортировать модули в порядке зависимостей
        /// </summary>
        private static IList<ModuleViewData> SortByDependencies(IList<ModuleViewData> modules)
        {
            return modules
                .Select(moduleView => moduleView.ModuleInfo)
                .OrderByTopology(moduleInfo => moduleInfo.Dependencies, true)
                .Select(moduleInfo => modules.First(modView => modView.ModuleInfo == moduleInfo))
                .ToArray();
        }

        /// <summary>
        ///     Перевести модули
        /// </summary>
        public void Translate(IList<ModuleViewData> modules)
        {
            Translator translator = new();

            this.Process = new MultiProcess
            {
                StartStatus = "Общий прогресс...",
                EndStatus = "Общий прогресс окончен",
                Processes =
                {
                    ProcessManager.Backup(modules),
                    ProcessManager.Recode(modules),
                    ProcessManager.Translate(modules, translator)
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