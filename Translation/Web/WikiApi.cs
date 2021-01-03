using System;
using System.Collections.Generic;
using System.IO;
using Translation.Transliteration;
using Translation.Web.Queries;

namespace Translation.Web
{
    /// <summary>
    ///     API к википедии
    /// </summary>
    public static class WikiApi
    {
        private static readonly string LogPath =
            Path.Combine(@"..\..\..\WikiLog", DateTime.Now.ToString("yyyyMMddTHHmmss"));

        private static readonly QueueTimer queueTimer = new QueueTimer(1000);
        private static readonly LangLinks langLinks = new LangLinks(WikiApi.queueTimer, WikiApi.LogPath);


        private static readonly PrefixSearch prefixSearch = new PrefixSearch(WikiApi.queueTimer, WikiApi.LogPath);

        private static readonly Search search = new Search(WikiApi.queueTimer, WikiApi.LogPath);

        public static IEnumerable<WordInLangs> GetTranslations(IEnumerable<string> words)
        {
            return WikiApi.langLinks.Execute(words);
        }

        public static List<string> PrefixSearch(string text)
        {
            return WikiApi.prefixSearch.Execute(text);
        }

        public static List<string> Search(string text)
        {
            return WikiApi.search.Execute(text);
        }
    }
}