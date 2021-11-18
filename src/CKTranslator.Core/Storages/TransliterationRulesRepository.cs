using CKTranslator.Core.Translation.Transliteration;

using System.Collections.Generic;

namespace CKTranslator.Core.Storages
{
    public class TransliterationRulesRepository : DictionaryRepository<TransliterationRule, string, string?[]>
    {
        protected override KeyValuePair<string, string?[]> Item2KeyValuePair(TransliterationRule item)
        {
            return new(item.Source, item.Target);
        }

        protected override TransliterationRule KeyValuePair2Item(KeyValuePair<string, string?[]> keyValuePair)
        {
            return new(keyValuePair.Key, keyValuePair.Value);
        }
    }
}