using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Core.Web.Queries
{
    public class Search : Query<string, List<string>>
    {
        public Search(QueueTimer? queryTimer, string logPath)
            : base(queryTimer, logPath)
        {
        }

        protected override string CreateRequest(string param)
        {
            return "https://en.wikipedia.org/w/api.php?action=query&format=json&list=search&utf8=1&srsearch="
                   + HttpUtility.UrlEncode(param)
                   + "&srlimit=500&srprop=";
        }

        protected override List<string> ParseResponse(string response)
        {
            JObject o = JObject.Parse(response);

            return o["query"]["search"]
                .Select(s => (string) s["title"])
                .ToList();
        }
    }
}