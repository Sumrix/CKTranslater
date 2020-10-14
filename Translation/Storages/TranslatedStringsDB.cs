using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Translation.Storages
{
    public class TranslatedStringsDB : 
        BaseDB<Dictionary<string, Dictionary<string, string>>>, 
        IEnumerable<TranslatedStringsDB.Item>
    {
        public class Item
        {
            public string eng;
            public IEnumerable<(string path, string rus)> Translations;
        }

        public static TranslatedStringsDB Load(string fileName = null)
        {
            return TranslatedStringsDB.LoadFromFile<TranslatedStringsDB>(fileName ?? FileName.TranslatedStringsDB);
        }

        public void Add(string eng, string rus, string path)
        {
            if (this.data.TryGetValue(eng, out Dictionary<string, string> rusValues))
            {
                if (!rusValues.ContainsKey(path) &&
                    (rusValues.Count > 1 || rusValues.Values.First() != rus))
                {
                    rusValues[path] = rus;
                }
            }
            else
            {
                this.data[eng] = new Dictionary<string, string> { { path, rus } };
            }
        }

        public string Get(string eng, string path)
        {
            string translation = null;

            if (this.data.TryGetValue(eng, out Dictionary<string, string> translations))
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
            return this.data.ContainsKey(eng);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return this.data
                .Select(item => new Item
                {
                    eng = item.Key,
                    Translations = item.Value.Select(kvp => (kvp.Key, kvp.Value))
                })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
