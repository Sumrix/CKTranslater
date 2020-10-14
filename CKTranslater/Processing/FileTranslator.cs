using CKTranslater.Parsing;
using CKTranslater.Storages;
using System.Linq;

namespace CKTranslater.Processing
{
    public class FileTranslator<TLoader, TParser>
        where TLoader : IValuesLoader, new()
        where TParser : IFileParser, new()
    {
        private readonly IValuesLoader rusLoader;
        private readonly IValuesLoader engLoader;

        public FileTranslator()
        {
            this.rusLoader = new TLoader
            {
                Language = Language.Rus
            };
            this.engLoader = new TLoader
            {
                Language = Language.Eng
            };
        }

        public Event Translate(string fileName)
        {
            TParser parser = new TParser();
            ScriptParseResult result = parser.Translate(fileName, this.TranslateString);
            return this.ProcessParseResult(result, fileName);
        }

        private Event ProcessParseResult(ScriptParseResult result, string fileName)
        {
            if (result.Errors?.Count > 0)
            {
                return new Event
                {
                    FileName = fileName,
                    Type = EventType.Error,
                    Desctiption = string.Join("\n", result.Errors)
                };
            }

            if (result.Strings?.Count > 0)
            {
                return new Event
                {
                    FileName = fileName,
                    Type = EventType.Info,
                    Desctiption = string.Join("\n", result.Strings)
                };
            }

            return null;
        }

        public void FillDictionary()
        {
            foreach (var eng in this.engLoader.Strings)
            {
                if (eng.Value.Any(IsRusLetter))
                {
                    continue;
                }

                if (this.rusLoader.Strings.TryGetValue(eng.Key, out string rusValue) &&
                    rusValue != eng.Value && rusValue.Any(IsRusLetter))
                {
                    DB.TranslatedStrings.Add(eng.Value, rusValue, eng.Key.Path);
                }
                else
                {
                    DB.NotTranslatedStrings.Add(eng.Value);
                }
            }
        }

        private string TranslateString(ScriptString @string)
        {
            string translation = DB.TranslatedStrings.Get(@string.Value, @string.Key.Path);

            if (translation != null)
            {
                IdManager.Transleated.AddLast(translation);
            }
            else
            {
                IdManager.NotTransleated.AddLast(@string.Value);
            }

            return translation;
        }

        public Event LoadRus(FileContext context)
        {
            return this.rusLoader.Load(context);
        }

        public Event LoadEng(FileContext context)
        {
            return this.engLoader.Load(context);
        }

        public void Clean()
        {
            this.rusLoader.Strings = this.rusLoader.Strings
                .Where(x => !IdManager.IgnoreValues.Contains(x.Value))
                .ToDictionary(x => x.Key, x => x.Value);

            this.engLoader.Strings = this.engLoader.Strings
                .Where(x => !IdManager.IgnoreValues.Contains(x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static bool IsRusLetter(char letter)
        {
            return 'а' <= letter && letter <= 'я' || 'А' <= letter && letter <= 'Я';
        }
    }
}
