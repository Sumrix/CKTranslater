using System;
using System.Collections.Generic;
using System.Linq;
using Core.Translation.Graphemes;

namespace Core.Translation.Transliteration
{
    /// <summary>
    ///     Правило транслитерации
    /// </summary>
    public class TransliterationRule
    {
        public readonly string Source;
        public string?[] Target;

        public TransliterationRule(string source, string?[] target)
        {
            this.Source = source;
            this.Target = target;
        }

        public static List<TransliterationRule> Create(GraphemeStatistic statistic)
        {
            var rules = new List<TransliterationRule>();
            foreach (var srcLetter in statistic.Values)
            {
                TranslationOccurrences?[]? occurrences = null;
                foreach (var trgLetter in srcLetter.Value)
                {
                    occurrences ??= new TranslationOccurrences[trgLetter.Value.Length];
                    for (int flags = 0; flags < trgLetter.Value.Length; flags++)
                    {
                        TranslationOccurrences? o = occurrences[flags];
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

                string source = srcLetter.Key;
                string?[] target = (occurrences ?? Array.Empty<TranslationOccurrences?>())
                    .Select(o => o?.Translation)
                    .ToArray();
                rules.Add(new TransliterationRule(source, target));
            }

            return rules;
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Source}:{this.Target.Length}";
        }

        private class TranslationOccurrences
        {
            public int Occurrences;
            public string? Translation;
        }
    }
}