using System.IO;

namespace CKTranslator.Processing
{
    public static class Backuping
    {
        public static Event Backup(FileContext context)
        {
            string folderName = Path.GetFileName(context.ModInfo.Path);
            string relatedFilePath = context.FullFileName.Substring(context.ModInfo.Path.Length + 1);
            string bakupFileName = Path.Combine(Path.Combine(FileManager.BackupPath, folderName), relatedFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(bakupFileName));
            if (!File.Exists(bakupFileName))
            {
                File.Copy(context.FullFileName, bakupFileName, true);
            }

            return null;
        }

        public static Event Restore(FileContext context)
        {
            string relativeFilePath = context.FullFileName.Substring(FileManager.BackupPath.Length + 1);
            string restoredFileName = Path.Combine(Path.GetDirectoryName(context.ModInfo.Path), relativeFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(restoredFileName));
            File.Copy(context.FullFileName, restoredFileName, true);

            return null;
        }
    }
}