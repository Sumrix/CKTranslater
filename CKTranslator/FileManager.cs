using CKTranslator.Parsing;
using CKTranslator.Processing;
using CKTranslator.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CKTranslator
{
    /// <summary>
    /// Функционал по работе с файлами модов
    /// </summary>
    public class FileManager
    {
        public static string[] LoadOrder;
        public static string[] ForbiddenFolders;
        public const string BackupPath = @"..\..\..\Data\Backups\";
        private static readonly string[] allowedExtensions = { ".txt", ".csv" };

        static FileManager()
        {
            LoadOrder = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x) &&
                            !x.StartsWith("#") &&
                            !x.StartsWith("-"))
                .ToArray();

            ForbiddenFolders = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.TrimStart())
                .Where(x => x.StartsWith("-"))
                .Select(x => x.Substring(1).TrimStart())
                .ToArray();
        }

        public static ICollection<FileContext> GetModFiles(ModInfo mod)
        {
            return Directory
                .EnumerateFiles(mod.Path, "*", SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Contains(System.IO.Path.GetExtension(file)) &&
                               !ForbiddenFolders.Any(folder => string.Compare(file, mod.Path.Length + 1, folder, 0, folder.Length) == 0))
                .Select(file => new
                {
                    OrderNum = Array.FindIndex(LoadOrder, folder => string.Compare(file, mod.Path.Length + 1, folder, 0, folder.Length) == 0),
                    FullFileName = file
                })
                .Where(x => x.OrderNum >= 0)
                .OrderBy(x => x.OrderNum)
                .Select(x => new FileContext
                {
                    ModInfo = mod,
                    ModFolder = System.IO.Path.GetDirectoryName(x.FullFileName.Substring(mod.Path.Length + 1)),
                    FullFileName = x.FullFileName
                })
                .ToArray();
        }

        public static ICollection<FileContext> GetBackupFiles(ModInfo mod)
        {
            string dirName = System.IO.Path.GetFileName(mod.Path);
            string bakupDirPath = System.IO.Path.Combine(BackupPath, dirName);

            if (!Directory.Exists(bakupDirPath))
            {
                return null;
            }

            return Directory.
                GetFiles(bakupDirPath, "*", SearchOption.AllDirectories)
                .Select(file => new FileContext
                {
                    ModInfo = mod,
                    ModFolder = System.IO.Path.GetDirectoryName(file.Substring(mod.Path.Length + 1)),
                    FullFileName = file
                })
                .ToArray();
        }

        public static ICollection<ModInfo> LoadMods()
        {
            ScriptParser parser = new ScriptParser();
            string[] fileNames = Directory.GetFiles(Resources.ModsPath, "*.mod");

            Dictionary<string, ModInfo> mods = fileNames
                .Select(file =>
                {
                    ModInfo mod = parser.ParseMod(file);
                    mod.Path = System.IO.Path.Combine(Resources.ModsPath, mod.Path.Substring(4));
                    if (!mod.IsArchive)
                    {
                        int? modFilesCount = FileManager.GetModFiles(mod)?.Count;
                        int? backupFilesCount = FileManager.GetBackupFiles(mod)?.Count;
                        mod.HasScripts = modFilesCount > 0;
                        mod.IsBackuped =  backupFilesCount >= modFilesCount;
                    }
                    return mod;
                })
                .ToDictionary(mod => mod.Name);

            foreach (ModInfo mod in mods.Values)
            {
                mod.Dependencies =
                    mod.Dependencies
                    ?.Select(dependence =>
                    {
                        mods.TryGetValue(dependence.Name, out ModInfo result);
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
