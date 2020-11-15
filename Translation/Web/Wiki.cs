using Translation.Transliteration;
using System.Collections.Generic;
using Translation.Web.Queries;

namespace Translation.Web
{
    /// <summary>
    /// API к википедии
    /// </summary>
    public static class Wiki
    {
        public static int RequiestCount = 0;

        private static readonly QueueTimer queueTimer = new QueueTimer(1000);
        private static readonly PrefixSearch prefixSearch = new PrefixSearch(queueTimer);
        private static readonly LangLinks langLinks = new LangLinks(queueTimer);

        public static List<string> GetSimilar(string text)
        {
            Wiki.RequiestCount++;
            return Wiki.prefixSearch.Execute(text);
        }
        public static IEnumerable<WordInLangs> GetTranslations(IEnumerable<string> words)
        {
            Wiki.RequiestCount++;
            return Wiki.langLinks.Execute(words);
        }
    }
}
