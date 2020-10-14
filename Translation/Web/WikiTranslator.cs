using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MoreLinq;

using Translation.Transliteration;
using Translation;
using Translation.Matching;

namespace Web
{
    /// <summary>
    /// Переводчик через википедию
    /// </summary>
    public static class WikiTranslator
    {
        public static IEnumerable<WordInLangs> Translate(IEnumerable<string> texts, Language language0, Language language1)
        {
            var exactTrans = TranslateExact(texts);

            var (goodTrans, failedTrans) = exactTrans
                .Partition(trans => trans.Lang2Word != null &&
                                    trans.Lang2Word.Any(IsRusLetter)
                );

            var failedTexts = failedTrans
                .Select(trans => trans.Lang1Word);

            var roughTrans = TranslateRough(failedTexts, language0, language1);

            return roughTrans
                .Union(goodTrans)
                .Select(WikiTranslator.Normalize);
        }

        public static IEnumerable<WordInLangs> TranslateExact(IEnumerable<string> words) =>
            words.Any()
            ? Wiki.GetTranslations(words).Select(WikiTranslator.Normalize)
            : Array.Empty<WordInLangs>();

        //public static IEnumerable<Translation> TranslateRough(IEnumerable<string> texts) => 
        //    from string text in texts
        //    let similars = Wiki.GetSimilar(text)
        //                       .Where(s => s.Contains(text))
        //    let roughTrans = WikiTranslator.TranslateExact(similars)
        //                                   .FirstOrDefault()
        //    select new Translation(text, roughTrans?.Result);

        public static IEnumerable<WordInLangs> TranslateRough(IEnumerable<string> texts, Language language0, Language language1)
        {
            var data = texts
                .Select(text => new
                {
                    Text = text,
                    Similars = Wiki.GetSimilar(text)
                                  .Where(s => s.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                  .ToArray()
                })
                .ToList();

            var allSimilars = data
                .SelectMany(s => s.Similars);

            var allRoughTrans = WikiTranslator.TranslateExact(allSimilars).ToArray();

            return allRoughTrans
                .Batch(data.Select(x => x.Similars.Length))
                .Zip(data)
                .Select(x => WikiTranslator.GetMostSuitableTranslation(x.Second.Text, x.First, language0, language1))
                .Select(WikiTranslator.Normalize)
                .ToArray();
        }

        private static WordInLangs GetMostSuitableTranslation(string original, IEnumerable<WordInLangs> translations,
            Language language0, Language language1)
        {
            var mostSuitableTranslation = translations
                .Where(x => x.Lang1Word.Contains(original) && !string.IsNullOrEmpty(x.Lang2Word))
                .SelectMany(x => x.Lang2Word.Split(c => !char.IsLetter(c), cs => string.Concat(cs)))
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(word => (word, WordMatch.Create(original, word, language0, language1).Similarity))
                .MaxBy(x => x.Similarity)
                .FirstOrDefault();

            return new WordInLangs(original, mostSuitableTranslation.word);
        }

        private static bool IsRusLetter(char letter) =>
               'а' <= letter && letter <= 'я'
            || 'А' <= letter && letter <= 'Я';

        private static WordInLangs Normalize(WordInLangs trans) =>
            new WordInLangs(
                trans.Lang1Word.ToLower(),
                trans.Lang2Word == null
                    ? null
                    : removePattern.Replace(trans.Lang2Word, "").ToLower()
            );

        private static readonly Regex removePattern = new Regex("(\\(.*\\) ?)");
    }
}
