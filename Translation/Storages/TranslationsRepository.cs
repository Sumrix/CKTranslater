using System.Collections.Generic;
using Translation.Transliteration;

namespace Translation.Storages
{
    public class TranslationsRepository : DictionaryRepository<WordInLangs, string, string>
    {
        protected override WordInLangs KeyValuePair2Item(KeyValuePair<string, string> keyValuePair)
        {
            return new WordInLangs(keyValuePair.Key, keyValuePair.Value);
        }

        protected override KeyValuePair<string, string> Item2KeyValuePair(WordInLangs item)
        {
            return new KeyValuePair<string, string>(item.Lang1Word, item.Lang2Word);
        }
    }
}