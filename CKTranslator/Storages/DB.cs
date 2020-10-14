namespace CKTranslator.Storages
{
    public static class DB
    {
        private static TranslatedStringsDB translatedStrings;
        private static NotTranslatedStringsDB notTranslatedStrings;

        public static TranslatedStringsDB TranslatedStrings => translatedStrings ??= TranslatedStringsDB.Load();
        public static NotTranslatedStringsDB NotTranslatedStrings => notTranslatedStrings ??= NotTranslatedStringsDB.Load();

        public static void Save()
        {
            translatedStrings?.Save();
            notTranslatedStrings?.Save();
        }
    }
}
