using System.Collections.Generic;

namespace CKTranslater.Parsing
{
    public class ScriptParseResult
    {
        public List<string> Errors;
        public List<ScriptString> Strings;
        public List<ScriptArray> Arrays;
    }

    public class ScriptString
    {
        public ScriptKey Key { get; set; }
        public string Value { get; set; }

        public ScriptString()
        {
            this.Key = new ScriptKey();
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

        public override string ToString()
        {
            return string.Format("{0} = \"{1}\"", this.Key, this.Value);
        }
    }

    public class ScriptArray
    {
        public ScriptKey Key { get; set; }
        public string[] Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0} = {{{1}}}", this.Key, string.Join(" ", this.Value));
        }
    }
}
