using System.Collections.Generic;
using Translation.Graphemes;

namespace Translation.Parsing
{
    // Маленькие графемы собираются в большие, если уже такие есть
    public class TranslationLengthCorrector
    {
        private int offset;

        // Дерево существующих графем
        private GraphemeVariant root;

        public static TranslationLengthCorrector Create(IEnumerable<GraphemeTranslation> translations,
            Language srcLanguage)
        {
            int variantCount = srcLanguage.MaxLetter - srcLanguage.MinLetter + 1;

            TranslationLengthCorrector t = new TranslationLengthCorrector
            {
                root = new GraphemeVariant { Variants = new GraphemeVariant[variantCount] },
                offset = srcLanguage.MinLetter
            };

            foreach (GraphemeTranslation translation in translations)
            {
                if (string.IsNullOrEmpty(translation.Original.Letters))
                {
                    continue;
                }

                ref var vs = ref t.root.Variants;
                GraphemeVariant v = null;

                foreach (char letter in translation.Original.Letters)
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

                v.ExistGrapheme = true;
            }

            return t;
        }

        /// <summary>
        ///     Объединить графемы, если есть такая возможность
        /// </summary>
        /// <param name="translations"></param>
        /// <returns></returns>
        public IEnumerable<GraphemeTranslation> Correct(IReadOnlyList<GraphemeTranslation> translations)
        {
            // Берём графемы и пробуем с ними пройти по дереву букв/графем.
            // Возвращяем наидлиннейший путь который смогли пройти.
            GraphemeVariant variant = this.root;
            GraphemeTranslation mergedTranslation = new GraphemeTranslation();
            int savedTranslationNum = 0;
            GraphemeTranslation savedMergedTranslation = null;
            bool savedButNotReturned = false;

            for (int translationNum = 0; translationNum < translations.Count; translationNum++)
            {
                GraphemeTranslation nextTranslation = translations[translationNum];

                string letters = nextTranslation.Original.Letters;
                mergedTranslation.MergeWith(nextTranslation);

                for (int letterNum = 0; letterNum < letters.Length; letterNum++)
                {
                    // Если дальше проходить по дереву графем нельзя - возвращяем последнее успешное
                    if (variant.Variants == null)
                    {
                        Load();
                        yield return savedMergedTranslation;
                        savedButNotReturned = false;
                        break;
                    }

                    // Делаем следующий шаг по дереву графем
                    char letter = letters[letterNum];
                    variant = variant.Variants[letter - this.offset];

                    // Если дальше проходить по дереву графем нельзя - возвращяем последнее успешное
                    if (variant == null)
                    {
                        Load();
                        yield return savedMergedTranslation;
                        savedButNotReturned = false;
                        break;
                    }

                    // Нашли успешное объединение - сохраняем
                    if (letterNum == letters.Length - 1 && variant.ExistGrapheme)
                    {
                        Save();
                        savedButNotReturned = true;
                    }
                }

                // Сохраняем успешное объединение графем
                void Save()
                {
                    savedTranslationNum = translationNum;
                    savedMergedTranslation = mergedTranslation.Clone();
                }

                // Возвращяем сохранённое успешное объединение графем
                void Load()
                {
                    translationNum = savedTranslationNum;
                    variant = this.root;
                    mergedTranslation = new GraphemeTranslation();
                }
            }

            // Если есть успешное объединение графем которое не вернули - возвращяем
            if (savedButNotReturned)
            {
                yield return savedMergedTranslation;
            }
        }
    }
}