namespace CKTranslator.Core.Models
{
    public class Document
    {
        public Document(string fullFileName, string rootPath)
        {
            FullFileName = fullFileName;
            RootPath = rootPath;
        }

        public string FullFileName { get; }

        public string RootPath { get; }
    }
}