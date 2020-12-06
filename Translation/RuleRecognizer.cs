using System.Collections.Generic;
using System.Linq;

using Translation.Graphemes;
using Translation.Interpolation;
using Translation.Matching;
using Translation.Parsing;
using Translation.Transliteration;

namespace Translation
{
    /// <summary>
    /// Класс создаёт правила перевода по существующему словарю
    /// </summary>
    public static class RuleRecognizer
    {
        public static List<TransliterationRule> Recognize(
            Language sourceLanguage,
            Language resultLanguage,
            IEnumerable<WordInLangs> wordTextTranslations)
        {
            List<List<GraphemeTranslation>> wordGraphemeTranslations =
                RuleRecognizer.SplitWordsIntoGraphemes(sourceLanguage, resultLanguage, wordTextTranslations);

            var g = wordGraphemeTranslations
                .Where(l => l.Last().Original.Letters == "h")
                .ToList();

            List<GraphemeTranslation> correctedGraphemeTranslations =
                RuleRecognizer.CorrectGraphemeLengths(sourceLanguage, wordGraphemeTranslations);

            List<TransliterationRule> rules =
                RuleRecognizer.CreateTranslationRules(correctedGraphemeTranslations);

            return rules;
        }

        private static List<List<GraphemeTranslation>> SplitWordsIntoGraphemes(
            Language language1,
            Language language2,
            IEnumerable<WordInLangs> wordTextTranslations)
        {
            List<List<GraphemeTranslation>> results = new List<List<GraphemeTranslation>>();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            //IEnumerable<string> lines = wordTextTranslations
            //    .Select(w => new
            //    {
            //        Match = WordMatch.Create(w.Lang1Word, w.Lang2Word, language1, language2),
            //        Word1 = w.Lang1Word,
            //        Word2 = w.Lang2Word
            //    })
            //    .Where(x => x.Match.Similarity >= 0.68f)
            //    .OrderBy(x => x.Match.Similarity)
            //    .Select(x => $"{x.Word1} - {x.Word2} - {x.Similarity}");

            //System.IO.File.WriteAllText(@"D:\Desktop\results.txt", string.Join("\r\n", lines));

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
                Interpolater.Interpolate(rule.Target, Bit.OnesCount((uint)rule.Target.Length - 1));
            }

            return rules;
        }
    }
}
