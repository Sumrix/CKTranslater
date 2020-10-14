using System.Collections;
using System.Collections.Generic;

namespace Translation.Storages
{
    public class NotTranslatedStringsDB : BaseDB<HashSet<string>>, IEnumerable<string>
    {
        public static NotTranslatedStringsDB Load(string fileName = null)
        {
            return NotTranslatedStringsDB.LoadFromFile<NotTranslatedStringsDB>(fileName ?? FileName.NotTranslatedStringsDB);
        }

        public void Add(string eng)
        {
            this.data.Add(eng);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Remove(string eng)
        {
            this.data.Remove(eng);
        }
    }
}
