using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CKTranslator.Core.Models
{
    public class Module
    {
        private static readonly string[] allowedExtensions = { ".txt", ".csv" };
        private static readonly string[] forbiddenFolders;
        private static readonly string[] loadOrder;

        static Module()
        {
            loadOrder = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x) &&
                            !x.StartsWith("#") &&
                            !x.StartsWith("-"))
                .ToArray();

            forbiddenFolders = File.ReadAllLines(FileName.LoadOrder)
                .Select(x => x.TrimStart())
                .Where(x => x.StartsWith("-"))
                .Select(x => x[1..].TrimStart())
                .ToArray();
        }

        public Module[] Dependencies { get; set; } = Array.Empty<Module>();

        public bool IsArchive { get; set; }

        public bool IsBackuped { get; set; }

        public bool IsRecoded { get; set; }

        public bool IsTranslated { get; set; }

        public string Name { get; set; } = "";

        public string Path { get; set; } = "";

        public List<Document> GetBackupFiles()
        {
            string dirName = System.IO.Path.GetFileName(Path);
            string backupDirPath = System.IO.Path.Combine(FileName.BackupPath, dirName);

            if (!Directory.Exists(backupDirPath))
            {
                return new();
            }

            return Directory.GetFiles(backupDirPath, "*", SearchOption.AllDirectories)
                .Select(file => new Document
                (
                    rootPath: System.IO.Path.GetDirectoryName(file[(Path.Length + 1)..]),
                    fullFileName: file
                ))
                .ToList();
        }

        public List<Document> GetModuleFiles()
        {
            return Directory
                .EnumerateFiles(Path, "*", SearchOption.AllDirectories)
                .Where(file =>
                    allowedExtensions.Contains(System.IO.Path.GetExtension(file)) &&
                    forbiddenFolders.All(folder =>
                        string.Compare(file, Path.Length + 1, folder, 0, folder.Length) != 0))
                .Select(file => new
                {
                    OrderNum = Array.FindIndex(loadOrder,
                        folder => string.Compare(file, Path.Length + 1, folder, 0, folder.Length) == 0),
                    FullFileName = file
                })
                .Where(x => x.OrderNum >= 0)
                .OrderBy(x => x.OrderNum)
                .Select(x => new Document
                (
                    rootPath: System.IO.Path.GetDirectoryName(x.FullFileName.Substring(Path.Length + 1)),
                    fullFileName: x.FullFileName
                ))
                .ToList();
        }
    }
}