using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MoreLinq;

using Translation.Transliteration;
using Translation.Matching;

namespace Translation.Web
{
    /// <summary>
    /// Переводчик через википедию
    /// </summary>
    public static class Wiki
    {
        /// <summary>
        /// Перевод строк lines из языка language0 в language1
        /// </summary>
        /// <param name="lines">Строки</param>
        /// <param name="sourceLanguage">Исходный язык перевода</param>
        /// <param name="targetLanguage">Целевой язык перевода</param>
        /// <returns>Переводы</returns>
        public static IEnumerable<WordInLangs> Translate(IEnumerable<string> lines, Language sourceLanguage, Language targetLanguage)
        {
            // То что можем, переводим точно
            var (exactTranslasions, notTranslatedLines) = Wiki.TranslateExact(lines)
                .Partition(
                    trans => trans.Lang2Word != null &&
                             targetLanguage.Belongs(trans.Lang2Word),
                    (True, False) => (
                        True.ToList(),
                        False.Select(x => x.Lang1Word).ToList()
                    )
                );

            // Там где нет точных переводов, ищем примерные
            var (foundResults, notFoundLines) = Wiki.Search(notTranslatedLines)
                .Partition(
                    result => result.Results.Any(), 
                    (True, False) => (
                        True.ToList(),
                        False.Select(x => x.Line).ToList()
                    )
                );

            // WikiApi.GetTranslations() разделяет поток данных на равные куски и отправляет
            // http-запросами на сайт. У нас щас куски все разной длины, и при разделении некоторые
            // блоки будут полупустыми, а количество запросов будет больше. По этому для экономии
            // объединим всё в один список, тогда все блоки будут полными и запросов будет меньше.
            var allResults = foundResults.SelectMany(s => s.Results);
            var allResultTrans = Wiki.TranslateExact(allResults).ToList();

            var originalsByResult = foundResults
                .SelectMany(foundResult => 
                    foundResult.Results.Select(result => (foundResult.Line, result))
                )
                .ToLookup(x => x.result, x => x.Line);
            var resultTrans = allResultTrans
                .SelectMany(wordInLangs => 
                    originalsByResult[wordInLangs.Lang1Word]
                        .Select(original => (original, wordInLangs))
                )
                .GroupBy(x => x.original, x => x.wordInLangs)
                .ToDictionary(x => x.Key);
            var roughTrans = resultTrans
                .Select(result => Wiki.GetMostSuitableTranslation(result.Key, result.Value, sourceLanguage, targetLanguage))
                .Select(Wiki.Normalize)
                .ToList();

            return roughTrans
                .Union(exactTranslasions)
                .Union(notFoundLines
                    .Select(line => new WordInLangs(line, null))
                )
                .Select(Wiki.Normalize);
        }

        /// <summary>
        /// Точный перевод
        /// </summary>
        /// <param name="lines">Строки для перевода</param>
        /// <returns>Точные переводы</returns>
        public static IEnumerable<WordInLangs> TranslateExact(IEnumerable<string> lines) 
            => lines?.Any() != true
                ? Array.Empty<WordInLangs>()
                : WikiApi.GetTranslations(lines)
                    .Select(Wiki.Normalize);

        /// <summary>
        /// Примерный перевод
        /// </summary>
        /// <param name="lines">Строки для перевода</param>
        /// <param name="sourceLanguage">Исходный язык перевода</param>
        /// <param name="targetLanguage">Целевой язык перевода</param>
        /// <returns>Примерные переводы</returns>
        public static List<(string Line, List<string> Results)> Search(IEnumerable<string> lines)
        {
            return lines
                .Select(line =>
                (
                    line,
                    WikiApi.PrefixSearch(line)
                        .Select(s => Normalize(s))
                        .Where(s => s.IndexOf(line, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        .ToList()
                ))
                .ToList();
        }

        private static WordInLangs GetMostSuitableTranslation(string line, IEnumerable<WordInLangs> translations,
            Language language0, Language language1)
        {
            var mostSuitableTranslation = translations
                // Берём подходящие пары слово-перевод
                .Where(x => x.Lang1Word.Contains(line, StringComparison.InvariantCultureIgnoreCase)
                            && !string.IsNullOrEmpty(x.Lang2Word))
                // Разбиваем перевод на слова
                .SelectMany(x => x.Lang2Word.Split(c => !char.IsLetter(c), cs => string.Concat(cs)))
                // Выбираем адекватные переводы
                .Where(translation => !string.IsNullOrEmpty(translation) && language1.Belongs(translation))
                // Выбираем наиболее подходящее слово
                .Select(translation => (translation, WordMatch.Create(line, translation, language0, language1).Similarity))
                .MaxBy(x => x.Similarity)
                .FirstOrDefault();

            return new WordInLangs(line, mostSuitableTranslation.translation);
        }

        private static WordInLangs Normalize(WordInLangs trans) =>
            new WordInLangs(
                Normalize(trans.Lang1Word),
                trans.Lang2Word == null
                    ? null
                    : removePattern.Replace(trans.Lang2Word, "").ToLower()
            );

        private static string Normalize(string line) => line.ToLower();

        private static readonly Regex removePattern = new Regex("(\\(.*\\) ?)");
    }
}
