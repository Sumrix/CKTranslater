using System.Collections.Generic;
using System.Linq;
using System.Text;
using Translation.Graphemes;
using Translation.Interpolation;
using Translation.Matching;
using Translation.Parsing;
using Translation.Transliteration;

namespace Translation
{
    /// <summary>
    ///     Класс создаёт правила перевода по существующему словарю
    /// </summary>
    public static class RuleRecognizer
    {
        public static List<TransliterationRule> Recognize(
            Language sourceLanguage,
            Language resultLanguage,
            IEnumerable<WordInLangs> wordTextTranslations)
        {
            List<List<GraphemeTranslation>> wordGraphemeTranslations =
                SplitWordsIntoGraphemes(sourceLanguage, resultLanguage, wordTextTranslations);

            List<GraphemeTranslation> correctedGraphemeTranslations =
                CorrectGraphemeLengths(sourceLanguage, wordGraphemeTranslations);

            List<TransliterationRule> rules =
                CreateTranslationRules(correctedGraphemeTranslations);

            return rules;
        }

        private static List<List<GraphemeTranslation>> SplitWordsIntoGraphemes(
            Language language1,
            Language language2,
            IEnumerable<WordInLangs> wordTextTranslations)
        {
            return (
                from t in wordTextTranslations
                let match = WordMatch.Create(t.Lang1Word, t.Lang2Word, language1, language2)
                where match.Similarity >= 0.68f
                select GraphemeTranslation.Convert(match.LetterMatches, language1.ToGraphemes(t.Lang1Word))
            ).ToList();
        }

        private static List<GraphemeTranslation> CorrectGraphemeLengths(
            Language srcLanguage,
            List<List<GraphemeTranslation>> wordGraphemeTranslations)
        {
            List<GraphemeTranslation> graphemeTranslations = wordGraphemeTranslations
                .SelectMany(t => t)
                .ToList();
            TranslationLengthCorrector lengthCorrector =
                TranslationLengthCorrector.Create(graphemeTranslations, srcLanguage);
            List<GraphemeTranslation> correctedGraphemeTranslations = wordGraphemeTranslations
                .SelectMany(t => lengthCorrector.Correct(t))
                .ToList();
            return correctedGraphemeTranslations;
        }

        private static List<TransliterationRule> CreateTranslationRules(
            List<GraphemeTranslation> correctedGraphemeTranslations)
        {
            GraphemeStatistic statistic = GraphemeStatistic.Create(correctedGraphemeTranslations);
            var k = statistic.Values.Where(x => x.Key == "h").FirstOrDefault();
            List<TransliterationRule> rules = TransliterationRule.Create(statistic);
            foreach (TransliterationRule rule in rules)
            {
                Interpolater.Interpolate(rule.Target, Bit.OnesCount((uint) rule.Target.Length - 1));
            }

            return rules;
        }
    }
}