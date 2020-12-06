using System.Collections;
using System.Collections.Generic;

using Translation.Transliteration;

namespace Translation.Storages
{
    public class RulesDB : BaseDB<Dictionary<string, string[]>>, IEnumerable<TransliterationRule>
    {
        public static RulesDB Load(string fileName = null)
        {
            return RulesDB.LoadFromFile<RulesDB>(fileName ?? FileName.RulesDB);
        }

        public void AddRange(List<TransliterationRule> rules)
        {
            foreach (TransliterationRule rule in rules)
            {
                this.data[rule.Source] = rule.Target;
            }
        }

        public IEnumerator<TransliterationRule> GetEnumerator()
        {
            foreach (var item in this.data)
            {
                yield return new TransliterationRule
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
