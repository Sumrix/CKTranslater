using System.Collections.Generic;
using System.Linq;

using Translation.Transliteration;

namespace Translation.Storages
{
    public class TranslatedDB : BaseDB<Dictionary<string, string>>
    {
        public IEnumerable<WordInLangs> WordsInLangs => this.data
            .Select(kvp => new WordInLangs(kvp.Key, kvp.Value));

        public static TranslatedDB Load(string fileName = null)
        {
            return TranslatedDB.LoadFromFile<TranslatedDB>(fileName ?? FileName.TranslatedDB);
        }

        public void AddRange(IEnumerable<WordInLangs> wordsInLangs)
        {
            foreach (WordInLangs wordInLangs in wordsInLangs)
            {
                this.data[wordInLangs.Lang1Word] = wordInLangs.Lang2Word;
            }
        }

        public void Add(WordInLangs wordInLangs)
        {
            this.data[wordInLangs.Lang1Word] = wordInLangs.Lang2Word;
        }

        public IEnumerable<WordInLangs> GetTranslations(IEnumerable<string> words)
        {
            foreach (string word in words)
            {
                yield return this.data.TryGetValue(word, out string translation)
                    ? new WordInLangs(word, translation)
                    : new WordInLangs(word, null);
            }
        }

        public string GetTranslation(string word)
        {
            return this.data.TryGetValue(word, out string translation)
                ? translation
                : null;
        }

        public void Clear()
        {
            this.data.Clear();
        }
    }
}
