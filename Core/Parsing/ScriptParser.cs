using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Core.Parsing.Listeners;
using Core.Processing;

namespace Core.Parsing
{
    public class ScriptParser : IFileParser
    {
        private readonly EncodingDetector decoder = new EncodingDetector();
        private readonly Encoding win1251;
        private readonly Encoding win1252;

        public ScriptParser()
        {
            this.win1251 = Encoding.GetEncoding("windows-1251");
            this.win1252 = Encoding.GetEncoding("windows-1252");
        }

        public ScriptParseResult Parse(FileContext context)
        {
            using StreamReader reader = new StreamReader(context.FullFileName, this.win1251);

            ErrorSaver errorSaver = new ErrorSaver();
            AntlrInputStream input = new AntlrInputStream(reader);
            CKLexer lexer = new CKLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            CKParser parser = new CKParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorSaver);

            StringsListener listener = new StringsListener();
            listener.Folder = context.ModFolder;
            ParseTreeWalker walker = new ParseTreeWalker();
            walker.Walk(listener, parser.script());

            return new ScriptParseResult
            {
                Errors = errorSaver.Messages,
                Strings = listener.Strings,
                Arrays = listener.Arrays
            };
        }

        public ScriptParseResult Translate(string fileName, StringTranslateHandle translator)
        {
            ErrorSaver errorSaver = new ErrorSaver();
            TranslateListener listener;

            using (StreamReader reader = new StreamReader(fileName, this.win1251))
            {
                AntlrInputStream input = new AntlrInputStream(reader);
                CKLexer lexer = new CKLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                CKParser parser = new CKParser(tokens);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(errorSaver);

                ParseTreeWalker walker = new ParseTreeWalker();
                listener = new TranslateListener(tokens, translator);
                walker.Walk(listener, parser.script());
            }

            File.WriteAllText(fileName, listener.Rewriter.GetText(), this.win1251);

            return new ScriptParseResult
            {
                Errors = errorSaver.Messages,
                Strings = listener.Strings,
                Arrays = listener.Arrays
            };
        }

        public ModInfo ParseMod(string fileName)
        {
            using StreamReader reader = new StreamReader(fileName, this.win1252);

            ErrorSaver errorSaver = new ErrorSaver();
            AntlrInputStream input = new AntlrInputStream(reader);
            CKLexer lexer = new CKLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            CKParser parser = new CKParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorSaver);

            ModDescriptionListener listener = new ModDescriptionListener();
            ParseTreeWalker walker = new ParseTreeWalker();
            walker.Walk(listener, parser.script());

            string name = listener.Strings.Find(s => s.Key.Path.LastStep == "name")?.Value;
            string path = listener.Strings.Find(s => s.Key.Path.LastStep == "path")?.Value;
            string archive = listener.Strings.Find(s => s.Key.Path.LastStep == "archive")?.Value;
            string[] dependences = listener.Arrays.Find(a => a.Key.Path.LastStep == "dependencies")?.Value;

            if (name == null || path == null && archive == null)
            {
                return null;
            }

            return new ModInfo
            {
                Name = name,
                Path = path ?? archive,
                IsArchive = archive != null,
                Dependencies = dependences
                                   ?.Select(name => new ModInfo { Name = name })
                                   .ToArray()
                               ?? new ModInfo[0]
            };
        }
    }
}