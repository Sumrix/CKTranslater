using CKTranslator.Core.Models;

using System.Collections.Generic;

namespace CKTranslator.Core.Processing
{
    public interface IModuleProcess
    {
        bool IsProcessable(Module module) => true;

        void ProcessDocument(Document document);

        IList<Document> SelectDocuments(Module module) => module.GetModuleFiles();
    }
}