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

        public static ICollection<FileContext> GetBackupFiles(ModInfo mod)
        {
            string dirName = Path.GetFileName(mod.Path);
            string backupDirPath = Path.Combine(FileManager.BackupPath, dirName);

            if (!Directory.Exists(backupDirPath))
            {
                return Array.Empty<FileContext>();
            }

            return Directory.GetFiles(backupDirPath, "*", SearchOption.AllDirectories)
                .Select(file => new FileContext
                {
                    ModInfo = mod,
                    ModFolder = Path.GetDirectoryName(file.Substring(mod.Path.Length + 1)) ?? "",
                    FullFileName = file
                })
                .ToArray();
        }

        public static ICollection<FileContext> GetModFiles(ModInfo mod)
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

        public static ICollection<ModInfo> LoadMods(string modsPath)
        {
            ScriptParser parser = new();
            string[] fileNames = Directory.GetFiles(modsPath, "*.mod");

            var mods = fileNames
                .Select(file =>
                {
                    ModInfo mod = parser.ParseMod(file);
                    mod.Path = Path.Combine(modsPath, mod.Path.Substring(4));
                    if (!mod.IsArchive)
                    {
                        int? modFilesCount = FileManager.GetModFiles(mod)?.Count;
                        int? backupFilesCount = FileManager.GetBackupFiles(mod)?.Count;
                        mod.HasScripts = modFilesCount > 0;
                        mod.IsBackupped = backupFilesCount >= modFilesCount;
                    }

                    return mod;
                })
                .ToDictionary(mod => mod.Name);

            foreach (ModInfo mod in mods.Values)
            {
                mod.Dependencies =
                    mod.Dependencies.Select(dependence =>
                        {
                            mods.TryGetValue(dependence.Name, out ModInfo? result);
                            return result;
                        })
                        .OfType<ModInfo>()
                        .ToArray();
            }

            return mods.Values
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}