using System.Collections.Generic;
using Translation.Graphemes;

namespace Translation.Transliteration
{
    /// <summary>
    /// Переводчик
    /// </summary>
    public class Translator
    {
        private GraphemeVariant[] tree;
        private Language srcLanguage;
        private int offset;

        public static Translator Create(IEnumerable<TranslationRule> rules, Language srcLanguage)
        {
            int variantCount = srcLanguage.MaxLetter - srcLanguage.MinLetter + 1;

            Translator t = new Translator
            {
                tree = new GraphemeVariant[variantCount],
                srcLanguage = srcLanguage,
                offset = srcLanguage.MinLetter
            };

            foreach (TranslationRule rule in rules)
            {
                ref GraphemeVariant[] vs = ref t.tree;
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
            GraphemeVariant[] vs = this.tree;
            GraphemeVariant v;
            string graphemeTranslation = "";
            // Перевод
            string translation = "";
            Grapheme grapheme = new Grapheme(GraphemeType.Silent, "");

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
