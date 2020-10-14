using System.Collections;
using System.Collections.Generic;

namespace CKTranslator.Storages
{
    public class NotTranslatedStringsDB : BaseDB<HashSet<string>>, IEnumerable<string>
    {
        public static NotTranslatedStringsDB Load(string fileName = null)
        {
            return NotTranslatedStringsDB.LoadFromFile<NotTranslatedStringsDB>(fileName ?? FileName.NotTranslatedStringsDB);
        }

        public void Add(string eng)
        {
            this.items.Add(eng);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Remove(string eng)
        {
            this.items.Remove(eng);
        }
    }
}
