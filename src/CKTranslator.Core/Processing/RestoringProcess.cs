using CKTranslator.Core.Models;

using System.Collections.Generic;
using System.IO;

namespace CKTranslator.Core.Processing
{
    public class RestoringProcess : IModuleProcess
    {
        public bool IsProcessable(Module module) => module.IsBackuped;

        public void ProcessDocument(Document document)
        {
            string relativeFilePath = document.FullFileName[(FileName.BackupPath.Length + 1)..];
            string restoredFileName = Path.Combine(Path.GetDirectoryName(document.RootPath), relativeFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(restoredFileName));
            File.Copy(document.FullFileName, restoredFileName, true);
        }

        public IList<Document> SelectDocuments(Module module) => module.GetBackupFiles();
    }
}