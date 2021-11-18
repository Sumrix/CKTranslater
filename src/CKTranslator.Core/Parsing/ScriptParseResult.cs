using System.Collections.Generic;

namespace CKTranslator.Core.Parsing
{
    public class ScriptArray
    {
        public ScriptKey Key { get; init; }
        public string[] Value { get; init; }

        public override string ToString()
        {
            return $"{this.Key} = {{{string.Join(" ", this.Value)}}}";
        }
    }

    public class ScriptParseResult
    {
        public List<ScriptArray> Arrays { get; init; } = new();
        public List<string> Errors { get; init; } = new();
        public List<ScriptString> Strings { get; init; } = new();
    }

    public class ScriptString
    {
        public ScriptString()
        {
            this.Key = new ScriptKey();
            this.Value = string.Empty;
        }

        public ScriptString(string key, string value)
        {
            this.Key = new ScriptKey
            {
                Path = new Path(new List<string> { key }),
                RepetitionIndex = 0
            };
            this.Value = value;
        }

        public ScriptKey Key { get; init; }
        public string Value { get; init; }

        public override string ToString()
        {
            return $"{this.Key} = \"{this.Value}\"";
        }
    }
}