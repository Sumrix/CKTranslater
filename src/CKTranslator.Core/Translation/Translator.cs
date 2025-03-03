﻿using CKTranslator.Core.Storages;
using CKTranslator.Core.Translation.Transliteration;
using CKTranslator.Core.Web;

using MoreLinq;

using System.Collections.Generic;
using System.Linq;

namespace CKTranslator.Core.Translation
{
    /// <summary>
    ///     Переводчик слов
    /// </summary>
    public static class Translator
    {
        /// <summary>
        ///     Перевести слова
        /// </summary>
        /// <param name="englishWords"></param>
        /// <returns></returns>
        public static IEnumerable<WordInLangs> Translate(IEnumerable<string> englishWords)
        {
            // Достаём переводы из кэша
            var (cacheMisses, cacheHits) = Translator.GetTranslationsFromCache(englishWords);
            var (cacheTranslated, cacheNotTranslated) = cacheHits.Partition(w => w.IsTranslated);

            // То что не перевелось, переводим через интернет
            var (webMisses, webHits) = Translator.GetTranslationsFromWeb(cacheMisses);

            // Переведённое через интернет сохраняем в кэш
            Translator.CacheTranslations(webMisses, webHits);

            // Транслитерируем не переведённое
            var toTransliterate = webMisses.Union(cacheNotTranslated.Select(w => w.Lang1Word));
            var transliterations = Translator.Transliterate(toTransliterate);

            // Объединяем результаты в один список
            return cacheTranslated.Union(webHits).Union(transliterations);
        }

        private static void CacheTranslations(IEnumerable<string> failedTranslations,
                    IEnumerable<WordInLangs> successfulTranslations)
        {
            Db.Translations.AddRange(successfulTranslations);
            Db.WebTranslationMisses.AddRange(failedTranslations);
            Db.Save();
        }

        /// <summary>
        ///     Получить данные по переводам из кэша
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private static (IEnumerable<string> misses, IEnumerable<WordInLangs> hits) GetTranslationsFromCache(
            IEnumerable<string> words)
        {
            var hits = new List<WordInLangs>();
            var misses = new List<string>();

            foreach (string word in words)
            {
                // Проверить наличие слов в БД переводов.
                if (Db.Translations.TryGetValue(word, out string? translation))
                {
                    hits.Add(new WordInLangs(word, translation));
                }
                else
                {
                    // Проверить наличие слов в БД без переводов
                    // Буквы через wiki не переводить, они не переведутся
                    if (Db.WebTranslationMisses.Contains(word) || word.Length == 1)
                    {
                        hits.Add(new WordInLangs(word, null));
                    }
                    else
                    {
                        misses.Add(word);
                    }
                }
            }

            return (misses, hits);
        }

        /// <summary>
        ///     Получить данные по переводам из интернета
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private static (List<string> misses, List<WordInLangs> hits) GetTranslationsFromWeb(
            IEnumerable<string> words)
        {
            var hits = new List<WordInLangs>();
            var misses = new List<string>();

            Language language0 = Language.Load(Db.EngLetters);
            Language language1 = Language.Load(Db.RusLetters);

            // Wiki перевод
            var wikiTranslations = Wiki.Translate(words, language0, language1).ToList();
            foreach (WordInLangs wordInLangs in wikiTranslations)
            {
                if (wordInLangs.Lang2Word != null)
                {
                    hits.Add(wordInLangs);
                }
                else
                {
                    misses.Add(wordInLangs.Lang1Word);
                }
            }

            return (misses, hits);
        }

        private static IEnumerable<WordInLangs> Transliterate(IEnumerable<string> toTransliterateWords)
        {
            Language language0 = Language.Load(Db.EngLetters);
            Language language1 = Language.Load(Db.RusLetters);

            // Производим обучение переводчика
            var wordsToLearn = Db.Translations
                .Where(w => !w.Lang1Word.Contains(" ") && w.Lang2Word != null && !w.Lang2Word.Contains(" "));

            var rules = RuleRecognizer.Recognize(language0, language1, wordsToLearn);
            Db.TransliterationRules.AddRange(rules);
            Db.TransliterationRules.Save();

            // Производим транслитерацию списка слов отложенных для транслитерации
            Transliterator translator = Transliterator.Create(rules, language0);
            return toTransliterateWords
                .Select(word => new WordInLangs(word, translator.Translate(word)))
                .ToList();
        }
    }
}