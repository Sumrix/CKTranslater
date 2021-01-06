using System.Collections.Generic;

namespace Core.Storages
{
    public class EngToRusMapRepository : DictionaryRepository<(char eng, string rus), char, string>
    {
        protected override KeyValuePair<char, string> Item2KeyValuePair((char eng, string rus) item)
        {
            return new KeyValuePair<char, string>(item.eng, item.rus);
        }

        protected override (char eng, string rus) KeyValuePair2Item(KeyValuePair<char, string> keyValuePair)
        {
            return (keyValuePair.Key, keyValuePair.Value);
        }
    }
}