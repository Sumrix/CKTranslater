using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Translation.Storages
{
    public class EngToRusMapDB : BaseDB<Dictionary<char, string>>, IEnumerable<(char eng, string rus)>
    {
        public static EngToRusMapDB Load(string fileName = null)
        {
            return EngToRusMapDB.LoadFromFile<EngToRusMapDB>(fileName ?? FileName.EngToRusMap);
        }

        public void Add(char eng, string rus)
        {
            this.data[eng] = rus;
        }

        public IEnumerator<(char eng, string rus)> GetEnumerator()
        {
            return this.data
                .Select(item => (item.Key, item.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
