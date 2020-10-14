using System.Collections.Generic;

namespace Translation.Storages
{
    public class NotTranslatedDB : BaseDB<HashSet<string>>
    {
        public static NotTranslatedDB Load(string fileName = null)
        {
            return NotTranslatedDB.LoadFromFile<NotTranslatedDB>(fileName ?? FileName.NotTranslatedDB);
        }

        public bool Contains(string word)
        {
            return this.data.Contains(word);
        }

        public void Add(string word)
        {
            this.data.Add(word);
        }
    }
}
