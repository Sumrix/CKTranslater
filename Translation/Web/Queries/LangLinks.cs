using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;
using Translation;
using Translation.Transliteration;

namespace Web.Queries
{
    public class LangLinks : LimitedQuery<string, WordInLangs>
    {
        protected override int BatchSize => 50;

        protected override string CreateRequest(IEnumerable<string> input) =>
            @"https://en.wikipedia.org/w/api.php?action=query&format=json&uselang=rus&prop=langlinks&titles="
            + HttpUtility.UrlEncode(string.Join("|", input))
            + "&redirects=1&lllang=ru&lllimit=max";

        public LangLinks(QueueTimer queryTimer) : base(queryTimer)
        {
        }

        protected override IEnumerable<WordInLangs> ParseResult(string response)
        {
            JObject o = JObject.Parse(response);

            var query = o["query"];
            var redirects = (query["redirects"] ?? new JObject())
                .Select(r => new
                {
                    To = (string)r["to"],
                    From = (string)r["from"]
                })
                .GroupBy(r => r.To, r => r.From)
                .ToDictionary(g => g.Key, g => g);

            return query["pages"].Children()
                .SelectMany(ch =>
                {
                    JToken p = ((JProperty)ch).Value;
                    string engWord = (string)p["title"];
                    string rusWord = (string)p.SelectToken(@"langlinks[0].['*']");
                    if (redirects.TryGetValue(engWord, out var redirect))
                    {
                        return redirect.Select(r => new WordInLangs(r, rusWord));
                    }
                    return new[]
                    {
                        new WordInLangs(
                            engWord,
                            (string)p.SelectToken(@"langlinks[0].['*']")
                        )
                    };
                });
        }
    }
}
