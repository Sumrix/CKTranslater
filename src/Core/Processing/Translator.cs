using Core.Parsing;

namespace Core.Processing
{
    using ScriptTranslator = FileTranslator<ScriptValuesLoader, ScriptParser>;
    using LocalisationTranslator = FileTranslator<LocalisationLoader, LocalisationParser>;

    public class Translator
    {
        private readonly LocalisationTranslator localisationTranslator;
        private readonly ScriptTranslator scriptTranslator;

        public Translator()
        {
            this.scriptTranslator = new ScriptTranslator();
            this.localisationTranslator = new LocalisationTranslator();
        }

        public void FillDictionary()
        {
            this.scriptTranslator.FillDictionary();
            this.localisationTranslator.FillDictionary();

            //var toDelete = DB.NotTranslatedStrings
            //    .Where(eng => DB.TranslatedStrings.Contains(eng))
            //    .ToList();

            //foreach (string eng in toDelete)
            //{
            //    DB.NotTranslatedStrings.Remove(eng);
            //}
        }

        public Event LoadEngFiles(FileContext context)
        {
            return context.ModFolder == "localisation"
                ? this.localisationTranslator.LoadEng(context)
                : this.scriptTranslator.LoadEng(context);
        }

        public Event LoadRusFiles(FileContext context)
        {
            return context.ModFolder == "localisation"
                ? this.localisationTranslator.LoadRus(context)
                : this.scriptTranslator.LoadRus(context);
        }

        public Event? TranslateScript(FileContext context)
        {
            return context.ModFolder == "localisation"
                ? this.localisationTranslator.Translate(context.FullFileName)
                : this.scriptTranslator.Translate(context.FullFileName);
        }
    }
}