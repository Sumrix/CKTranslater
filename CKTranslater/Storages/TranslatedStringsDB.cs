using CKTranslater.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace CKTranslater.Storages
{
    public class TranslatedStringsDB : BaseDB<Dictionary<string, Dictionary<Path, string>>>
    {
        public static TranslatedStringsDB Load(string fileName = null)
        {
            return TranslatedStringsDB.LoadFromFile<TranslatedStringsDB>(fileName ?? FileName.TranslatedStringsDB);
        }

        public void Add(string eng, string rus, Path path)
        {
            if (this.items.TryGetValue(eng, out Dictionary<Path, string> rusValues))
            {
                if (!rusValues.ContainsKey(path) && 
                    (rusValues.Count > 1 || rusValues.Values.First() != rus))
                {
                    rusValues[path] = rus;
                }
            }
            else
            {
                this.items[eng] = new Dictionary<Path, string> { { path, rus } };
            }
        }

        public string Get(string eng, Path path)
        {
            string translation = null;

            if (this.items.TryGetValue(eng, out Dictionary<Path, string> translations))
            {
                if (translations.Count > 1)
                {
                    if (!translations.TryGetValue(path, out translation))
                    {
                        translation = translations.Values.First();
                    }
                }
                else
                {
                    translation = translations.Values.First();
                }
            }

            return translation;
        }

        public bool Contains(string eng)
        {
            return this.items.ContainsKey(eng);
        }
    }
}
