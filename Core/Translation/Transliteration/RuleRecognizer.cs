using System.Collections.Generic;
using System.Linq;
using Core.Graphemes;
using Core.Interpolation;
using Core.Matching;
using Core.Parsing;
using Core.Storages;

namespace Core.Transliteration
{
    /// <summary>
    ///     Создаёт правила перевода по существующему словарю
    /// </summary>
    public static class RuleRecognizer
    {
        private const float MinimumSimilarity = 0.68f;

        private static List<GraphemeTranslation> CorrectGraphemeLengths(
            Language srcLanguage,
            IReadOnlyCollection<IReadOnlyList<GraphemeTranslation>> wordGraphemeTranslations)
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
            IEnumerable<GraphemeTranslation> correctedGraphemeTranslations)
        {
            GraphemeStatistic statistic = GraphemeStatistic.Create(correctedGraphemeTranslations);
            List<TransliterationRule> rules = TransliterationRule.Create(statistic);
            foreach (TransliterationRule rule in rules)
            {
                Interpolater.Interpolate(rule.Target, Bit.OnesCount((uint) rule.Target.Length - 1));
            }

            return rules;
        }

        public static List<TransliterationRule> Recognize(
            Language sourceLanguage,
            Language resultLanguage,
            IEnumerable<WordInLangs> wordTextTranslations)
        {
            List<List<GraphemeTranslation>> wordGraphemeTranslations = (
                from t in wordTextTranslations
                let match = WordMatch.Create(t.Lang1Word, t.Lang2Word, sourceLanguage, resultLanguage)
                where match.Success
                select GraphemeTranslation.Create(match.LetterMatches, sourceLanguage.ToGraphemes(t.Lang1Word))
            ).ToList();

            List<GraphemeTranslation> correctedGraphemeTranslations =
                RuleRecognizer.CorrectGraphemeLengths(sourceLanguage, wordGraphemeTranslations);

            IEnumerable<GraphemeTranslation> allGraphemeTranslations = DB.EngToRusMap
                .Select(x => new GraphemeTranslation(sourceLanguage.ToGrapheme(x.eng.ToString()), x.rus))
                .Union(correctedGraphemeTranslations);

            List<TransliterationRule> rules =
                RuleRecognizer.CreateTranslationRules(allGraphemeTranslations);

            return rules;
        }
    }
}