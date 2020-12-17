using Translation.Transliteration;
using System.Collections.Generic;
using Translation.Web.Queries;

namespace Translation.Web
{
    /// <summary>
    /// API к википедии
    /// </summary>
    public static class WikiApi
    {
        public static int RequiestCount = 0;

        private static readonly QueueTimer queueTimer = new QueueTimer(1000);
        private static readonly PrefixSearch prefixSearch = new PrefixSearch(queueTimer);
        private static readonly LangLinks langLinks = new LangLinks(queueTimer);
        private static readonly Search search = new Search(queueTimer);

        public static List<string> PrefixSearch(string text)
        {
            WikiApi.RequiestCount++;
            return WikiApi.prefixSearch.Execute(text);
        }

        public static List<string> Search(string text)
        {
            WikiApi.RequiestCount++;
            return WikiApi.search.Execute(text);
        }

        public static IEnumerable<WordInLangs> GetTranslations(IEnumerable<string> words)
        {
            WikiApi.RequiestCount++;
            return WikiApi.langLinks.Execute(words);
        }
    }
}
