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
        private static TranslationsRepository translations;
        private static EngToRusMapRepository engToRusMap;
        private static LettersRepository engLetters;
        private static LettersRepository rusLetters;
        private static EngToRusSimilaritiesRepository engToRusSimilarities;

        public static EngToRusScriptLinesRepository EngToRusScriptLines => DB.engToRusScriptLines
            ??= Repository.Load<EngToRusScriptLinesRepository>(FileName.EngToRusScriptLines);

        public static TransliterationRulesRepository TransliterationRules => DB.transliterationRules
            ??= Repository.Load<TransliterationRulesRepository>(FileName.TransliterationRules);

        public static WebTranslationMissesRepository WebTranslationMisses => DB.webTranslationMisses
            ??= Repository.Load<WebTranslationMissesRepository>(FileName.WebTranslationMisses);

        public static TranslationsRepository Translations => DB.translations
            ??= Repository.Load<TranslationsRepository>(FileName.Translations);

        public static EngToRusMapRepository EngToRusMap => DB.engToRusMap
            ??= Repository.Load<EngToRusMapRepository>(FileName.EngToRusMap);

        public static LettersRepository EngLetters => DB.engLetters
            ??= Repository.Load<LettersRepository>(FileName.EngLetters);

        public static LettersRepository RusLetters => DB.rusLetters
            ??= Repository.Load<LettersRepository>(FileName.RusLetters);

        public static EngToRusSimilaritiesRepository EngToRusSimilarities => DB.engToRusSimilarities
            ??= Repository.Load<EngToRusSimilaritiesRepository>(FileName.EngToRusSimilarities);

        public static void Save()
        {
            DB.engToRusScriptLines?.Save();
            DB.transliterationRules?.Save();
            DB.webTranslationMisses?.Save();
            // Закоментировано, что бы не сохранять неправильные переводы, пока переводчик не будет работать хорошо
            //DB.translations?.Save();
            DB.engToRusMap?.Save();
            DB.engLetters?.Save();
            DB.rusLetters?.Save();
            DB.engToRusSimilarities?.Save();
        }
    }
}