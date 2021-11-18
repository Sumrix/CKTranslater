using CKTranslator.Core.Models;
using CKTranslator.Core.Parsing;
using CKTranslator.Core.Translation;

using System.Linq;

namespace CKTranslator.Core.Processing
{
    public class TranslatingProcess : IModuleProcess
    {
        public void ProcessDocument(Document document)
        {
            // if file is script
            switch (System.IO.Path.GetExtension(document.FullFileName))
            {
                case ".txt":
                    var scriptParser = new ScriptParser();
                    // TODO: Add translation method for one string argument
                    scriptParser.Translate(document.FullFileName, s => Translator.Translate(new string[] { s }).First().Lang2Word);
                    break;

                case ".csv":
                    var localisationParser = new LocalisationParser();
                    // TODO: Add translation method for one string argument
                    localisationParser.Translate(document.FullFileName, s => Translator.Translate(new string[] { s }).First().Lang2Word);
                    break;
            }
        }
    }
}