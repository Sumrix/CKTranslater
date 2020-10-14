using System.Collections.Generic;
using System.Linq;

using Translation.Graphemes;

namespace Translation.Transliteration
{
    /// <summary>
    /// Правило перевода
    /// </summary>
    public class TranslationRule
    {
        public string Source;
        public string[] Target;

        public TranslationRule()
        {
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode();
        }

        private class TranslationOccurrences
        {
            public string Translation;
            public int Occurrences;
        }

        public static List<TranslationRule> Create(GraphemeStatistic statistic)
        {
            var rules = new List<TranslationRule>();
            foreach (var srcLetter in statistic.Values)
            {
                TranslationOccurrences[] occurrences = null;
                foreach (var trgLetter in srcLetter.Value)
                {
                    occurrences ??= new TranslationOccurrences[trgLetter.Value.Length];
                    for (int flags = 0; flags < trgLetter.Value.Length; flags++)
                    {
                        TranslationOccurrences o = occurrences[flags];
                        if (o == null)
                        {
                            o = new TranslationOccurrences();
                            occurrences[flags] = o;
                        }
                        ref int occurrence = ref trgLetter.Value[flags];
                        if (occurrence > o.Occurrences)
                        {
                            o.Occurrences = occurrence;
                            o.Translation = trgLetter.Key;
                        }
                    }
                }
                rules.Add(new TranslationRule
                {
                    Source = srcLetter.Key,
                    Target = occurrences
                        .Select(o => o?.Translation)
                        .ToArray()
                });
            }

            return rules;
        }
    }
}
