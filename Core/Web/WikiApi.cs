using System;
using System.Collections.Generic;
using System.IO;
using Core.Transliteration;
using Core.Web.Queries;

namespace Core.Web
{
    /// <summary>
    ///     API к википедии
    /// </summary>
    public static class WikiApi
    {
        private static readonly string logPath =
            Path.Combine(@"..\..\..\WikiLog", DateTime.Now.ToString("yyyyMMddTHHmmss"));
        private static readonly QueueTimer queueTimer = null; //new QueueTimer(0);
        private static readonly LangLinks langLinks = new(WikiApi.queueTimer, WikiApi.logPath);
        private static readonly PrefixSearch prefixSearch = new(WikiApi.queueTimer, WikiApi.logPath);
        private static readonly Search search = new(WikiApi.queueTimer, WikiApi.logPath);

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