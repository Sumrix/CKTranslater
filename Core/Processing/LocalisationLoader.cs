using System.Collections.Generic;
using System.Text;
using Core.Parsing;

namespace Core.Processing
{
    public class LocalisationLoader : IValuesLoader
    {
        public static HashSet<string> Keys = new();
        private readonly LocalisationParser parser;

        public LocalisationLoader()
        {
            this.Strings = new Dictionary<ScriptKey, string>();
            this.parser = new LocalisationParser();
        }

        public Dictionary<ScriptKey, string> Strings { get; set; }
        public Language Language { get; set; }
        public Dictionary<ScriptKey, string[]> Arrays => null;

        public Event Load(FileContext context)
        {
            ScriptParseResult result = this.parser.Parse(context);

            StringBuilder desctiption = new();

            foreach (ScriptString @string in result.Strings)
            {
                @string.Key.Path.AddForward(context.ModFolder);
                this.Strings[@string.Key] = @string.Value;
                LocalisationLoader.Keys.Add(@string.Key.Path.LastStep);
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