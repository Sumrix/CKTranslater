using CKTranslator.Core.Translation.Graphemes;

using System.Collections.Generic;

namespace CKTranslator.Core.Translation.Transliteration
{
    /// <summary>
    ///     Траслитератор
    /// </summary>
    public class Transliterator
    {
        private readonly int offset;
        private readonly Language srcLanguage;
        private GraphemeVariant?[] tree;

        private Transliterator(int offset, Language srcLanguage, GraphemeVariant[] tree)
        {
            this.offset = offset;
            this.srcLanguage = srcLanguage;
            this.tree = tree;
        }

        /// <summary>
        ///     Создать новый транслитератор на основе правил транслитерации
        /// </summary>
        /// <param name="rules">Правила транслитерации</param>
        /// <param name="srcLanguage">Язык транслитерируемых слов</param>
        /// <returns></returns>
        public static Transliterator Create(IEnumerable<TransliterationRule> rules, Language srcLanguage)
        {
            int variantCount = srcLanguage.MaxLetter - srcLanguage.MinLetter + 1;

            Transliterator t = new(srcLanguage.MinLetter, srcLanguage, new GraphemeVariant[variantCount]);

            foreach (TransliterationRule rule in rules)
            {
                ref var vs = ref t.tree;
                GraphemeVariant? v = null;

                foreach (char letter in rule.Source)
                {
                    //vs ??= new GraphemeVariant[variantCount];
                    v = vs[letter - t.offset];
                    if (v == null)
                    {
                        v = new GraphemeVariant();
                        vs[letter - t.offset] = v;
                    }

                    vs = ref v.Variants;
                }

                if (v != null)
                {
                    v.Options = rule.Target;
                }
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
            string? graphemeTranslation = "";
            // Перевод
            string translation = "";
            Grapheme grapheme = new(GraphemeType.Silent, "");

            while (curPos <= maxPos)
            {
                grapheme.MergeWith(graphemes[curPos]);
                char letter = word[curPos];
                GraphemeVariant? v = vs[letter - this.offset];

                if (v != null)
                {
                    if (v.Options.Length > 0)
                    {
                        savedPos = curPos;
                        graphemeTranslation = v.Options[grapheme.Flags];
                    }

                    if (v.Variants.Length > 0 && curPos < maxPos)
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