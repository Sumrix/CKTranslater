using CKTranslater.Parsing;
using System.Collections.Generic;
using System.Text;

namespace CKTranslater.Processing
{
    public class LocalisationLoader : IValuesLoader
    {
        public Dictionary<ScriptKey, string> Strings { get; set; }
        public static HashSet<string> Keys = new HashSet<string>();
        public Language Language { get; set; }
        public Dictionary<ScriptKey, string[]> Arrays => null;
        private readonly LocalisationParser parser;

        public LocalisationLoader()
        {
            this.Strings = new Dictionary<ScriptKey, string>();
            this.parser = new LocalisationParser();
        }

        public Event Load(FileContext context)
        {
            ScriptParseResult result = this.parser.Parse(context);

            StringBuilder desctiption = new StringBuilder();

            foreach (ScriptString @string in result.Strings)
            {
                @string.Key.Path.AddForward(context.ModFolder);
                this.Strings[@string.Key] = @string.Value;
                Keys.Add(@string.Key.Path.LastStep);
                desctiption.AppendLine(@string.ToString());
            }

            if (desctiption.Length > 0)
            {
                return new Event
                {
                    FileName = context.FullFileName,
                    Type = EventType.Info,
                    Desctiption = desctiption.ToString()
                };
            }

            return null;
        }

    }
}
