using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Parsing;
using Core.Processing;
using Path = System.IO.Path;

namespace Core
{
    /// <summary>
    ///     Функционал по работе с файлами модов
    /// </summary>
    public class FileManager
    {
        public const string BackupPath = @"..\..\..\Data\Backups\";
        private static readonly string[] allowedExtensions = { ".txt", ".csv" };
        private static readonly string[] forbiddenFolders;
        private static readonly string[] loadOrder;

        static FileManager()
        {
            FileManager.loadOrder = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x) &&
                            !x.StartsWith("#") &&
                            !x.StartsWith("-"))
                .ToArray();

            FileManager.forbiddenFolders = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.TrimStart())
                .Where(x => x.StartsWith("-"))
                .Select(x => x[1..].TrimStart())
                .ToArray();
        }

        public static ICollection<FileContext> GetBackupFiles(ModuleInfo module)
        {
            string dirName = Path.GetFileName(module.Path);
            string backupDirPath = Path.Combine(FileManager.BackupPath, dirName);

            if (!Directory.Exists(backupDirPath))
            {
                return Array.Empty<FileContext>();
            }

            return Directory.GetFiles(backupDirPath, "*", SearchOption.AllDirectories)
                .Select(file => new FileContext
                {
                    ModInfo = module,
                    ModFolder = Path.GetDirectoryName(file.Substring(module.Path.Length + 1)) ?? "",
                    FullFileName = file
                })
                .ToArray();
        }

        public static ICollection<FileContext> GetModuleFiles(ModuleInfo mod)
        {
            return Directory
                .EnumerateFiles(mod.Path, "*", SearchOption.AllDirectories)
                .Where(file =>
                    FileManager.allowedExtensions.Contains(Path.GetExtension(file)) &&
                    FileManager.forbiddenFolders.All(folder =>
                        string.Compare(file, mod.Path.Length + 1, folder, 0, folder.Length) != 0))
                .Select(file => new
                {
                    OrderNum = Array.FindIndex(FileManager.loadOrder,
                        folder => string.Compare(file, mod.Path.Length + 1, folder, 0, folder.Length) == 0),
                    FullFileName = file
                })
                .Where(x => x.OrderNum >= 0)
                .OrderBy(x => x.OrderNum)
                .Select(x => new FileContext
                {
                    ModInfo = mod,
                    ModFolder = Path.GetDirectoryName(x.FullFileName.Substring(mod.Path.Length + 1)) ?? "",
                    FullFileName = x.FullFileName
                })
                .ToArray();
        }

        public static ICollection<ModuleInfo> LoadModules(string modulesPath)
        {
            ScriptParser parser = new();
            string[] fileNames = Directory.GetFiles(modulesPath, "*.mod");

            var modules = fileNames
                .Select(file =>
                {
                    ModuleInfo module = parser.ParseModule(file);
                    module.Path = Path.Combine(modulesPath, module.Path.Substring(4));
                    if (!module.IsArchive)
                    {
                        int? moduleFilesCount = FileManager.GetModuleFiles(module)?.Count;
                        int? backupFilesCount = FileManager.GetBackupFiles(module)?.Count;
                        module.HasScripts = moduleFilesCount > 0;
                        module.IsBackupped = backupFilesCount >= moduleFilesCount;
                    }

                    return module;
                })
                .ToDictionary(module => module.Name);

            foreach (ModuleInfo module in modules.Values)
            {
                module.Dependencies =
                    module.Dependencies.Select(dependence =>
                        {
                            modules.TryGetValue(dependence.Name, out ModuleInfo? result);
                            return result;
                        })
                        .OfType<ModuleInfo>()
                        .ToArray();
            }

            return modules.Values
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}