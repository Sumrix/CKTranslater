using CKTranslater.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CKTranslater.Processing
{
    public class ScriptValuesLoader : IValuesLoader
    {
        public Dictionary<ScriptKey, string> Strings { get; set; }
        public Dictionary<ScriptKey, string[]> Arrays { get; private set; }
        public Language Language { get; set; }

        public static HashSet<string> OtherKeys = new HashSet<string>();
        public static HashSet<string> IdKeys = new HashSet<string>();
        public static HashSet<string> StringKeys = new HashSet<string>();

        public ScriptValuesLoader()
        {
            this.Strings = new Dictionary<ScriptKey, string>();
            this.Arrays = new Dictionary<ScriptKey, string[]>();
        }

        public Event Load(FileContext context)
        {
            ScriptParser parser = new ScriptParser();
            ScriptParseResult result = parser.Parse(context);

            if (result.Errors.Count > 0)
            {
                return new Event
                {
                    FileName = context.FullFileName,
                    Type = EventType.Error,
                    Desctiption = string.Join("\n", result.Errors)
                };
            }

            StringBuilder desctiption = new StringBuilder();

            foreach (ScriptString @string in result.Strings)
            {
                if (this.Language != Language.Rus || @string.Value.Any(IsRusLetter))
                {
                    @string.Key.Path.AddForward(context.ModFolder);
                    this.Strings[@string.Key] = @string.Value;
                    desctiption.AppendLine(@string.ToString());
                }
            }

            //foreach (ScriptArray array in result.Arrays)
            //{
            //    if (this.Language != Language.Rus || array.Value.Any(x => x.Any(IsRusLetter)))
            //    {
            //        array.Key.Path.AddForward(context.ModFolder);
            //        this.Arrays.Add(array.Key, array.Value);
            //        desctiption.AppendLine(array.ToString());
            //    }
            //}

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

        private static bool IsRusLetter(char letter)
        {
            return 'а' <= letter && letter <= 'я' || 'А' <= letter && letter <= 'Я';
        }
    }
}
