using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Core.Parsing;

namespace Core.Processing
{
    public class LocalisationLoader : IValuesLoader
    {
        public static readonly HashSet<string> Keys = new();
        private readonly LocalisationParser parser;

        public LocalisationLoader()
        {
            this.Strings = new Dictionary<ScriptKey, string>();
            this.parser = new LocalisationParser();
        }

        public IDictionary<ScriptKey, string> Strings { get; set; }
        public Language Language { get; set; }
        public IDictionary<ScriptKey, string[]> Arrays => ImmutableDictionary<ScriptKey, string[]>.Empty;

        public Event? Load(FileContext context)
        {
            ScriptParseResult result = this.parser.Parse(context);

            StringBuilder description = new();

            foreach (ScriptString @string in result.Strings)
            {
                @string.Key.Path.AddForward(context.ModFolder);
                this.Strings[@string.Key] = @string.Value;
                LocalisationLoader.Keys.Add(@string.Key.Path.LastStep);
                description.AppendLine(@string.ToString());
            }

            if (description.Length > 0)
            {
                return new Event
                {
                    FileName = context.FullFileName,
                    Type = EventType.Info,
                    Description = description.ToString()
                };
            }

            return null;
        }
    }
}