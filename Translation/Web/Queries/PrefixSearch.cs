using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Translation.Web.Queries
{
    /// <summary>
    ///     Поиск по префиксу.
    /// </summary>
    public class PrefixSearch : Query<string, List<string>>
    {
        public PrefixSearch(QueueTimer queryTimer, string logPath)
            : base(queryTimer, logPath)
        {
        }

        protected override string CreateRequest(string input)
        {
            return "https://en.wikipedia.org/w/api.php?action=query&format=json&uselang=rus&list=prefixsearch&pssearch="
                   + HttpUtility.UrlEncode(input)
                   + "&pslimit=500";
        }

        protected override List<string> ParseResult(string response)
        {
            JObject o = JObject.Parse(response);

            return o["query"]["prefixsearch"]
                .Select(s => (string) s["title"])
                .ToList();
        }
    }
}