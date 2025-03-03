﻿using CKTranslator.Core.Translation.Transliteration;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKTranslator.Core.Web.Queries
{
    /// <summary>
    ///     В LangLinks есть ограничение на количество заголовков в одном запросе,
    ///     по этому наследуем функционал от LimitedQuery, который разделит входящие
    ///     данные на куски по 50 и выполнит отдельно.
    /// </summary>
    public class LangLinks : LimitedQuery<string, WordInLangs>
    {
        private const int MaxNumOfTitles = 50;

        public LangLinks(QueueTimer? queryTimer, string logPath)
            : base(queryTimer, LangLinks.MaxNumOfTitles, logPath)
        {
        }

        protected override string CreateRequest(IEnumerable<string> param)
        {
            return @"https://en.wikipedia.org/w/api.php?action=query&format=json&uselang=rus&prop=langlinks&titles="
                   + HttpUtility.UrlEncode(string.Join("|", param))
                   + "&redirects=1&lllang=ru&lllimit=max";
        }

        protected override IEnumerable<WordInLangs> ParseResponse(string response)
        {
            JObject o = JObject.Parse(response);

            JToken? query = o["query"];
            var redirects = (query?["redirects"] ?? new JObject())
                .Select(r => new
                {
                    To = (string?)r["to"],
                    From = (string?)r["from"]
                })
                .GroupBy(r => r.To, r => r.From)
                .ToDictionary(g => g.Key, g => g);

            return query?["pages"]
                ?.Children()
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
                            (string?) p.SelectToken(@"langlinks[0].['*']")
                        )
                    };
                }) ?? Enumerable.Empty<WordInLangs>();
        }
    }
}