using CKTranslator.Core.Models;

using System.IO;

namespace CKTranslator.Core.Processing
{
    public class BackupProcess : IModuleProcess
    {
        public void ProcessDocument(Document document)
        {
            string folderName = Path.GetFileName(document.RootPath);
            string relatedFilePath = document.FullFileName[(document.RootPath.Length + 1)..];
            string backupFileName = Path.Combine(Path.Combine(FileName.BackupPath, folderName), relatedFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(backupFileName));
            if (!File.Exists(backupFileName))
            {
                File.Copy(document.FullFileName, backupFileName, true);
            }
        }
    }
}