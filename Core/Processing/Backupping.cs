using System.IO;

namespace Core.Processing
{
    public static class Backupping
    {
        public static Event? Backup(FileContext context)
        {
            string folderName = Path.GetFileName(context.ModInfo.Path);
            string relatedFilePath = context.FullFileName[(context.ModInfo.Path.Length + 1)..];
            string backupFileName = Path.Combine(Path.Combine(FileManager.BackupPath, folderName), relatedFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(backupFileName));
            if (!File.Exists(backupFileName))
            {
                File.Copy(context.FullFileName, backupFileName, true);
            }

            return null;
        }

        public static Event? Restore(FileContext context)
        {
            string relativeFilePath = context.FullFileName.Substring(FileManager.BackupPath.Length + 1);
            string restoredFileName = Path.Combine(Path.GetDirectoryName(context.ModInfo.Path), relativeFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(restoredFileName));
            File.Copy(context.FullFileName, restoredFileName, true);

            return null;
        }
    }
}