using CKTranslator.Processing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CKTranslator.Parsing
{
    public class LocalisationParser : IFileParser
    {
        private readonly Regex regex;
        private readonly Encoding win1251;
        private readonly Encoding win1252;

        public LocalisationParser()
        {
            this.regex = new Regex(@"^([a-zA-Z0-9_]*?);(.*?);.*$");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.win1251 = Encoding.GetEncoding("windows-1251");
            this.win1252 = Encoding.GetEncoding("windows-1252");
        }

        public ScriptParseResult Parse(FileContext context)
        {
            List<ScriptString> strings = new List<ScriptString>();

            using (StreamReader reader = new StreamReader(context.FullFileName, this.win1251))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    Match match = this.regex.Match(line);
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value;

                        strings.Add(new ScriptString(key, value));
                        IdManager.AddValueToIgnore(key);
                    }
                }
            }

            return new ScriptParseResult
            {
                Strings = strings
            };
        }

        public ScriptParseResult Translate(string fileName, StringTranslateHandle translator)
        {
            ScriptParseResult result = new ScriptParseResult
            {
                Strings = new List<ScriptString>()
            };

            using (MemoryStream memory = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(memory, this.win1251))
            {
                using (StreamReader reader = new StreamReader(fileName, this.win1251))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Match match = this.regex.Match(line);
                        if (match.Success)
                        {
                            string key = match.Groups[1].Value;
                            string engValue = match.Groups[2].Value;

                            string rusValue = translator(new ScriptString(key, engValue));
                            if (rusValue != null)
                            {
                                writer.WriteLine($"{key};{rusValue};x");
                                result.Strings.Add(new ScriptString(key, rusValue));
                                continue;
                            }
                        }

                        writer.WriteLine(line);
                    }
                }

                writer.Flush();

                using (var fileStream = File.Create(fileName))
                {
                    memory.Seek(0, SeekOrigin.Begin);
                    memory.CopyTo(fileStream);
                }
            }

            return result;
        }
    }
}
