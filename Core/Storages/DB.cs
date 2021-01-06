namespace Core.Storages
{
    /// <summary>
    ///     Хранилища к которым осуществляется доступ в проекте
    /// </summary>
    public static class DB
    {
        private static LettersRepository engLetters;
        private static EngToRusMapRepository engToRusMap;
        private static EngToRusScriptLinesRepository engToRusScriptLines;
        private static EngToRusSimilaritiesRepository engToRusSimilarities;
        private static QueryCacheRepository queryCache;
        private static LettersRepository rusLetters;
        private static TranslationsRepository translations;
        private static TransliterationRulesRepository transliterationRules;
        private static WebTranslationMissesRepository webTranslationMisses;

        public static LettersRepository EngLetters => DB.engLetters
            ??= Repository.Load<LettersRepository>(FileName.EngLetters);

        public static EngToRusMapRepository EngToRusMap => DB.engToRusMap
            ??= Repository.Load<EngToRusMapRepository>(FileName.EngToRusMap);

        public static EngToRusScriptLinesRepository EngToRusScriptLines => DB.engToRusScriptLines
            ??= Repository.Load<EngToRusScriptLinesRepository>(FileName.EngToRusScriptLines);

        public static EngToRusSimilaritiesRepository EngToRusSimilarities => DB.engToRusSimilarities
            ??= Repository.Load<EngToRusSimilaritiesRepository>(FileName.EngToRusSimilarities);

        public static QueryCacheRepository QueryCache => DB.queryCache
            ??= Repository.Load<QueryCacheRepository>(FileName.QueryCache);

        public static LettersRepository RusLetters => DB.rusLetters
            ??= Repository.Load<LettersRepository>(FileName.RusLetters);

        public static TranslationsRepository Translations => DB.translations
            ??= Repository.Load<TranslationsRepository>(FileName.Translations);

        public static TransliterationRulesRepository TransliterationRules => DB.transliterationRules
            ??= Repository.Load<TransliterationRulesRepository>(FileName.TransliterationRules);

        public static WebTranslationMissesRepository WebTranslationMisses => DB.webTranslationMisses
            ??= Repository.Load<WebTranslationMissesRepository>(FileName.WebTranslationMisses);

        public static void Save()
        {
            DB.engLetters?.Save();
            DB.engToRusMap?.Save();
            DB.engToRusScriptLines?.Save();
            DB.engToRusSimilarities?.Save();
            DB.queryCache?.Save();
            DB.rusLetters?.Save();
            DB.translations?.Save();
            DB.transliterationRules?.Save();
            DB.webTranslationMisses?.Save();
        }
    }
}