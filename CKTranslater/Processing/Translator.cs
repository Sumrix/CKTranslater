using CKTranslater.Parsing;
using CKTranslater.Storages;
using System.Collections.Generic;


namespace CKTranslater.Processing
{
    using ScriptTranslator = FileTranslator<ScriptValuesLoader, ScriptParser>;
    using LocalisationTranslator = FileTranslator<LocalisationLoader, LocalisationParser>;

    public class Translator
    {
        private readonly ScriptTranslator scriptTranslator;
        private readonly LocalisationTranslator localisationTranslator;

        public Translator()
        {
            this.scriptTranslator = new ScriptTranslator();
            this.localisationTranslator = new LocalisationTranslator();
        }

        public void FillDictionary()
        {
            this.scriptTranslator.FillDictionary();
            this.localisationTranslator.FillDictionary();

            List<string> toDelete = new List<string>();

            foreach (string eng in DB.NotTranslatedStrings)
            {
                if (DB.TranslatedStrings.Contains(eng))
                {
                    toDelete.Add(eng);
                }
            }

            foreach (string eng in toDelete)
            {
                DB.NotTranslatedStrings.Remove(eng);
            }
        }

        public Event LoadRusFiles(FileContext context)
        {
            if (context.ModFolder == "localisation")
            {
                return this.localisationTranslator.LoadRus(context);
            }
            return this.scriptTranslator.LoadRus(context);
        }

        public Event LoadEngFiles(FileContext context)
        {
            if (context.ModFolder == "localisation")
            {
                return this.localisationTranslator.LoadEng(context);
            }
            return this.scriptTranslator.LoadEng(context);
        }

        public Event TranslateScript(FileContext context)
        {
            if (context.ModFolder == "localisation")
            {
                return this.localisationTranslator.Translate(context.FullFileName);
            }
            return this.scriptTranslator.Translate(context.FullFileName);
        }
    }
}
