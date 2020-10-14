using System.Collections;
using System.Collections.Generic;

using Translation.Transliteration;

namespace Translation.Storages
{
    public class RulesDB : BaseDB<Dictionary<string, string[]>>, IEnumerable<TranslationRule>
    {
        public static RulesDB Load(string fileName = null)
        {
            return RulesDB.LoadFromFile<RulesDB>(fileName ?? FileName.RulesDB);
        }

        public void AddRange(List<TranslationRule> rules)
        {
            foreach (TranslationRule rule in rules)
            {
                this.data[rule.Source] = rule.Target;
            }
        }

        public IEnumerator<TranslationRule> GetEnumerator()
        {
            foreach (var item in this.data)
            {
                yield return new TranslationRule
                {
                    Source = item.Key,
                    Target = item.Value
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
