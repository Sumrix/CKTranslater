using System.Collections.Generic;
using Core.Graphemes;

namespace Core.Transliteration
{
    /// <summary>
    ///     Траслитератор
    /// </summary>
    public class Transliterator
    {
        private int offset;
        private Language srcLanguage;
        private GraphemeVariant[] tree;

        /// <summary>
        ///     Создать новый транслитератор на основе правил транслитерации
        /// </summary>
        /// <param name="rules">Правила траслитерации</param>
        /// <param name="srcLanguage">Язык траслитерируемых слов</param>
        /// <returns></returns>
        public static Transliterator Create(IEnumerable<TransliterationRule> rules, Language srcLanguage)
        {
            int variantCount = srcLanguage.MaxLetter - srcLanguage.MinLetter + 1;

            Transliterator t = new()
            {
                tree = new GraphemeVariant[variantCount],
                srcLanguage = srcLanguage,
                offset = srcLanguage.MinLetter
            };

            foreach (TransliterationRule rule in rules)
            {
                ref var vs = ref t.tree;
                GraphemeVariant v = null;

                foreach (char letter in rule.Source)
                {
                    vs ??= new GraphemeVariant[variantCount];
                    v = vs[letter - t.offset];
                    if (v == null)
                    {
                        v = new GraphemeVariant();
                        vs[letter - t.offset] = v;
                    }

                    vs = ref v.Variants;
                }

                v.Options = rule.Target;
            }

            return t;
        }

        public string Translate(string word)
        {
            word = word.ToLower();
            // Графемы слова word
            var graphemes = this.srcLanguage.ToGraphemes(word);
            // Позиции в слове word
            int curPos = 0;
            int maxPos = graphemes.Count - 1;
            int savedPos = 0;
            // Узлы дерева парсинга
            var vs = this.tree;
            GraphemeVariant v;
            string graphemeTranslation = "";
            // Перевод
            string translation = "";
            Grapheme grapheme = new(GraphemeType.Silent, "");

            while (curPos <= maxPos)
            {
                grapheme.MergeWith(graphemes[curPos]);
                char letter = word[curPos];
                v = vs[letter - this.offset];

                if (v != null)
                {
                    if (v.Options != null)
                    {
                        savedPos = curPos;
                        graphemeTranslation = v.Options[grapheme.Flags.Value];
                    }

                    if (v.Variants != null && curPos < maxPos)
                    {
                        curPos++;
                        vs = v.Variants;
                        continue;
                    }
                }

                translation += graphemeTranslation;
                vs = this.tree;
                curPos = savedPos + 1;
                grapheme = new Grapheme(GraphemeType.Silent, "");
            }

            return translation;
        }
    }
}