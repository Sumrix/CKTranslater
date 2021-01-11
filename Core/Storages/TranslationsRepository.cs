using System.Collections.Generic;
using Core.Translation.Transliteration;

namespace Core.Storages
{
    public class TranslationsRepository : DictionaryRepository<WordInLangs, string, string?>
    {
        protected override KeyValuePair<string, string?> Item2KeyValuePair(WordInLangs item)
        {
            return new(item.Lang1Word, item.Lang2Word);
        }

        protected override WordInLangs KeyValuePair2Item(KeyValuePair<string, string?> keyValuePair)
        {
            return new(keyValuePair.Key, keyValuePair.Value);
        }
    }
}