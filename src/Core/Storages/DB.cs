namespace Core.Storages
{
    /// <summary>
    ///     Хранилища к которым осуществляется доступ в проекте
    /// </summary>
    public static class Db
    {
        private static LettersRepository? engLetters;
        private static EngToRusMapRepository? engToRusMap;
        private static EngToRusScriptLinesRepository? engToRusScriptLines;
        private static EngToRusSimilaritiesRepository? engToRusSimilarities;
        private static QueryCacheRepository? queryCache;
        private static LettersRepository? rusLetters;
        private static TranslationsRepository? translations;
        private static TransliterationRulesRepository? transliterationRules;
        private static WebTranslationMissesRepository? webTranslationMisses;

        public static LettersRepository EngLetters => Db.engLetters
            ??= Repository.Load<LettersRepository>(FileName.EngLetters);

        public static EngToRusMapRepository EngToRusMap => Db.engToRusMap
            ??= Repository.Load<EngToRusMapRepository>(FileName.EngToRusMap);

        public static EngToRusScriptLinesRepository EngToRusScriptLines => Db.engToRusScriptLines
            ??= Repository.Load<EngToRusScriptLinesRepository>(FileName.EngToRusScriptLines);

        public static EngToRusSimilaritiesRepository EngToRusSimilarities => Db.engToRusSimilarities
            ??= Repository.Load<EngToRusSimilaritiesRepository>(FileName.EngToRusSimilarities);

        public static QueryCacheRepository QueryCache => Db.queryCache
            ??= Repository.Load<QueryCacheRepository>(FileName.QueryCache);

        public static LettersRepository RusLetters => Db.rusLetters
            ??= Repository.Load<LettersRepository>(FileName.RusLetters);

        public static TranslationsRepository Translations => Db.translations
            ??= Repository.Load<TranslationsRepository>(FileName.Translations);

        public static TransliterationRulesRepository TransliterationRules => Db.transliterationRules
            ??= Repository.Load<TransliterationRulesRepository>(FileName.TransliterationRules);

        public static WebTranslationMissesRepository WebTranslationMisses => Db.webTranslationMisses
            ??= Repository.Load<WebTranslationMissesRepository>(FileName.WebTranslationMisses);

        public static void Save()
        {
            Db.engLetters?.Save();
            Db.engToRusMap?.Save();
            Db.engToRusScriptLines?.Save();
            Db.engToRusSimilarities?.Save();
            Db.queryCache?.Save();
            Db.rusLetters?.Save();
            Db.translations?.Save();
            Db.transliterationRules?.Save();
            Db.webTranslationMisses?.Save();
        }
    }
}