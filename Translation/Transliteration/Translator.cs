using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NameTranslation.Storages;
using NameTranslation.Web;

namespace NameTranslation.Transliteration
{
    /// <summary>
    ///     Переводчик слов
    /// </summary>
    public static class Translator
    {
        private static void CacheTranslations(IEnumerable<string> failedTranslations,
            IEnumerable<WordInLangs> successfulTranslations)
        {
            DB.Translations.AddRange(successfulTranslations);
            DB.WebTranslationMisses.AddRange(failedTranslations);
            DB.Save();
        }

        /// <summary>
        ///     Получить данные по переводам из кэша
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private static (IEnumerable<string> misses, IEnumerable<WordInLangs> hits) GetTranslationsFromCache(
            IEnumerable<string> words)
        {
            List<WordInLangs> hits = new List<WordInLangs>();
            List<string> misses = new List<string>();

            foreach (string word in words)
            {
                // Проверить наличие слов в БД переводов.
                if (DB.Translations.TryGetValue(word, out string translation))
                {
                    hits.Add(new WordInLangs(word, translation));
                }
                else
                {
                    // Проверить наличие слов в БД без переводов
                    // Буквы через wiki не переводить, они не переведутся
                    if (DB.WebTranslationMisses.Contains(word) || word.Length == 1)
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
        private static (IEnumerable<string> misses, IEnumerable<WordInLangs> hits) GetTranslationsFromWeb(
            IEnumerable<string> words)
        {
            if (!words.Any())
            {
                return (Enumerable.Empty<string>(), Enumerable.Empty<WordInLangs>());
            }

            List<WordInLangs> hits = new List<WordInLangs>();
            List<string> misses = new List<string>();

            Language language0 = Language.Load(DB.EngLetters);
            Language language1 = Language.Load(DB.RusLetters);

            // Wiki перевод
            List<WordInLangs> wikiTranslations = Wiki.Translate(words, language0, language1).ToList();
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

        /// <summary>
        ///     Перевести слова
        /// </summary>
        /// <param name="englishWords"></param>
        /// <returns></returns>
        public static IEnumerable<WordInLangs> Translate(IEnumerable<string> englishWords)
        {
            if (!englishWords.Any())
            {
                return Enumerable.Empty<WordInLangs>();
            }

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

        private static IEnumerable<WordInLangs> Transliterate(IEnumerable<string> toTransliterateWords)
        {
            Language language0 = Language.Load(DB.EngLetters);
            Language language1 = Language.Load(DB.RusLetters);

            // Производим обучение переводчика
            IEnumerable<WordInLangs> wordsToLearn = DB.Translations
                .Where(w => !w.Lang1Word.Contains(" ") && !w.Lang2Word.Contains(" "));

            List<TransliterationRule> rules = RuleRecognizer.Recognize(language0, language1, wordsToLearn);
            DB.TransliterationRules.AddRange(rules);
            DB.TransliterationRules.Save();

            // Производим транслитерацию списка слов отложенных для транслитерации
            Transliterator translator = Transliterator.Create(rules, language0);
            return toTransliterateWords
                .Select(word => new WordInLangs(word, translator.Translate(word)))
                .ToList();
        }
    }
}