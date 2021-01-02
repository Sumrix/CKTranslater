using System.Collections.Generic;
using Translation.Transliteration;

namespace Translation.Storages
{
    public class TransliterationRulesRepository : DictionaryRepository<TransliterationRule, string, string[]>
    {
        protected override TransliterationRule KeyValuePair2Item(KeyValuePair<string, string[]> keyValuePair)
        {
            return new TransliterationRule { Source = keyValuePair.Key, Target = keyValuePair.Value };
        }

        protected override KeyValuePair<string, string[]> Item2KeyValuePair(TransliterationRule item)
        {
            return new KeyValuePair<string, string[]>(item.Source, item.Target);
        }
    }
}