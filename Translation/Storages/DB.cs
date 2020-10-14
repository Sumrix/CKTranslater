namespace Translation.Storages
{
    /// <summary>
    /// Хранилища к которым осуществляется доступ в проекте
    /// </summary>
    public static class DB
    {
        private static TranslatedStringsDB translatedStrings;
        private static NotTranslatedStringsDB notTranslatedStrings;
        private static RulesDB rules;
        private static NotTranslatedDB notTranslated;
        private static TranslatedDB translated;
        private static EngToRusMapDB engToRusMap;
        private static LettersDB engLetters;
        private static LettersDB rusLetters;
        private static EngToRusSimilaritiesDB engToRusSimilarities;

        public static TranslatedStringsDB TranslatedStrings => translatedStrings ??= TranslatedStringsDB.Load();
        public static NotTranslatedStringsDB NotTranslatedStrings => notTranslatedStrings ??= NotTranslatedStringsDB.Load();
        public static RulesDB Rules => rules ??= RulesDB.Load();
        public static NotTranslatedDB NotTranslated => notTranslated ??= NotTranslatedDB.Load();
        public static TranslatedDB Translated => translated ??= TranslatedDB.Load();
        public static EngToRusMapDB EngToRusMap => engToRusMap ??= EngToRusMapDB.Load();
        public static LettersDB EngLetters => engLetters ??= LettersDB.Load(FileName.EngLanguage);
        public static LettersDB RusLetters => rusLetters ??= LettersDB.Load(FileName.RusLanguage);
        public static EngToRusSimilaritiesDB EngToRusSimilarities => engToRusSimilarities ??= EngToRusSimilaritiesDB.Load();

        public static void Save()
        {
            translatedStrings?.Save();
            notTranslatedStrings?.Save();
            rules?.Save();
            notTranslated?.Save();
            translated?.Save();
            engToRusMap?.Save();
            engLetters?.Save();
            rusLetters?.Save();
            engToRusSimilarities?.Save();
        }
    }
}
