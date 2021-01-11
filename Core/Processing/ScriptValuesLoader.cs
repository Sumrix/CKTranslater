using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Parsing;

namespace Core.Processing
{
    public class ScriptValuesLoader : IValuesLoader
    {
        public static HashSet<string> IdKeys = new();
        public static HashSet<string> OtherKeys = new();
        public static HashSet<string> StringKeys = new();

        public ScriptValuesLoader()
        {
            this.Strings = new Dictionary<ScriptKey, string>();
            this.Arrays = new Dictionary<ScriptKey, string[]>();
        }

        public IDictionary<ScriptKey, string> Strings { get; set; }
        public IDictionary<ScriptKey, string[]> Arrays { get; }
        public Language Language { get; set; }

        public Event? Load(FileContext context)
        {
            ScriptParser parser = new();
            ScriptParseResult result = parser.Parse(context);

            if (result.Errors.Count > 0)
            {
                return new Event
                {
                    FileName = context.FullFileName,
                    Type = EventType.Error,
                    Description = string.Join("\n", result.Errors)
                };
            }

            StringBuilder description = new();

            foreach (ScriptString @string in result.Strings)
            {
                if (this.Language != Language.Rus || @string.Value.Any(ScriptValuesLoader.IsRusLetter))
                {
                    @string.Key.Path.AddForward(context.ModFolder);
                    this.Strings[@string.Key] = @string.Value;
                    description.AppendLine(@string.ToString());
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

        private static bool IsRusLetter(char letter)
        {
            return 'а' <= letter && letter <= 'я' || 'А' <= letter && letter <= 'Я';
        }
    }
}