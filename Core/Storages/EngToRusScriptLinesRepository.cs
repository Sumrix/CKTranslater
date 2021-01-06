using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EngToRusScriptLinesData =
    System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace Core.Storages
{
    public class EngToRusScriptLine
    {
        public string EngLine;
        public IEnumerable<(string Path, string RusLine)> RusScriptLines;
    }

    /// <summary>
    ///     База данных найденных переводов строк из аналогичных английских и русских скриптов.
    ///     Для каждой английской строки может соответствовать много русских,
    ///     по этому вместе с переводом хранятся и пути по которым обнаружены переводы.
    /// </summary>
    public class EngToRusScriptLinesRepository : Repository, IEnumerable<EngToRusScriptLine>
    {
        private EngToRusScriptLinesData data = new EngToRusScriptLinesData();

        public IEnumerator<EngToRusScriptLine> GetEnumerator()
        {
            // Создаём удобную обёртку данных перед возвратом.
            // Удобнее будет писать item.EngLine, чем item.Key в случае с KeyValuePair.
            return this.data
                .Select(item => new EngToRusScriptLine
                {
                    EngLine = item.Key,
                    RusScriptLines = item.Value.Select(kvp => (kvp.Key, kvp.Value))
                })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(string engLine, string rusLine, string rusLinePath)
        {
            if (this.data.TryGetValue(engLine, out Dictionary<string, string> rusScriptLines))
            {
                if (!rusScriptLines.ContainsKey(rusLinePath) &&
                    (rusScriptLines.Count > 1 || rusScriptLines.Values.First() != rusLine))
                {
                    rusScriptLines[rusLinePath] = rusLine;
                }
            }
            else
            {
                this.data[engLine] = new Dictionary<string, string> { { rusLinePath, rusLine } };
            }
        }

        public bool Contains(string engLine)
        {
            return this.data.ContainsKey(engLine);
        }

        protected override object GetDataToSave()
        {
            // Перемещаем данные в сортированные коллекции, для упорядоченного хранения данных в файлах.
            // Чисто для красоты и удобного отслеживания изменений. Потом можно убрать.
            var sortedData = new SortedDictionary<string, SortedDictionary<string, string>>();
            foreach (var engToRusScriptLine in this.data)
            {
                var sortedRusScriptLines = new SortedDictionary<string, string>(engToRusScriptLine.Value);
                sortedData.Add(engToRusScriptLine.Key, sortedRusScriptLines);
            }

            return sortedData;
        }

        public string GetRusLine(string engLine, string rusLinePath)
        {
            string rusLine = null;

            if (!this.data.TryGetValue(engLine, out Dictionary<string, string> rusScriptLines))
            {
                return rusLine;
            }

            if (rusScriptLines.Count > 1)
            {
                if (!rusScriptLines.TryGetValue(rusLinePath, out rusLine))
                {
                    rusLine = rusScriptLines.Values.First();
                }
            }
            else
            {
                rusLine = rusScriptLines.Values.First();
            }

            return rusLine;
        }

        protected override void LoadData(string fileName)
        {
            this.data = JsonHelper.Deserialize<EngToRusScriptLinesData>(fileName);
        }
    }
}