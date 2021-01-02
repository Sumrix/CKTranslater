namespace Translation.Storages
{
    /// <summary>
    ///     Хранилища к которым осуществляется доступ в проекте
    /// </summary>
    public static class DB
    {
        private static EngToRusScriptLinesRepository engToRusScriptLines;
        private static TransliterationRulesRepository transliterationRules;
        private static WebTranslationMissesRepository webTranslationMisses;
        private static TranslationsRepository translated;
        private static EngToRusMapRepository engToRusMap;
        private static LettersRepository engLetters;
        private static LettersRepository rusLetters;
        private static EngToRusSimilaritiesRepository engToRusSimilarities;

        public static EngToRusScriptLinesRepository EngToRusScriptLines => engToRusScriptLines
            ??= Repository.Load<EngToRusScriptLinesRepository>(FileName.EngToRusScriptLines);

        public static TransliterationRulesRepository TransliterationRules => transliterationRules
            ??= Repository.Load<TransliterationRulesRepository>(FileName.TransliterationRules);

        public static WebTranslationMissesRepository WebTranslationMisses => webTranslationMisses
            ??= Repository.Load<WebTranslationMissesRepository>(FileName.WebTranslationMisses);

        public static TranslationsRepository Translations => translated
            ??= Repository.Load<TranslationsRepository>(FileName.Translations);

        public static EngToRusMapRepository EngToRusMap => engToRusMap
            ??= Repository.Load<EngToRusMapRepository>(FileName.EngToRusMap);

        public static LettersRepository EngLetters => engLetters
            ??= Repository.Load<LettersRepository>(FileName.EngLetters);

        public static LettersRepository RusLetters => rusLetters
            ??= Repository.Load<LettersRepository>(FileName.RusLetters);

        public static EngToRusSimilaritiesRepository EngToRusSimilarities => engToRusSimilarities
            ??= Repository.Load<EngToRusSimilaritiesRepository>(FileName.EngToRusSimilarities);

        public static void Save()
        {
            engToRusScriptLines?.Save();
            transliterationRules?.Save();
            webTranslationMisses?.Save();
            translated?.Save();
            engToRusMap?.Save();
            engLetters?.Save();
            rusLetters?.Save();
            engToRusSimilarities?.Save();
        }
    }
}