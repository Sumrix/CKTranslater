using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;

namespace Translation.Web.Queries
{
    public class Search : Query<string, List<string>>
    {
        public Search(QueueTimer queryTimer) : base(queryTimer)
        {
        }

        protected override string CreateRequest(string input) =>
            "https://en.wikipedia.org/w/api.php?action=query&format=json&list=search&utf8=1&srsearch="
            + HttpUtility.UrlEncode(input)
            + "&srlimit=500&srprop=";

        protected override List<string> ParseResult(string response)
        {
            JObject o = JObject.Parse(response);

            return o["query"]["search"]
                .Select(s => (string)s["title"])
                .ToList();
        }
    }
}
