using System;
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
        private readonly EncodingDetector decoder = new();
        private readonly Encoding win1251;
        private readonly Encoding win1252;

        public ScriptParser()
        {
            this.win1251 = Encoding.GetEncoding("windows-1251");
            this.win1252 = Encoding.GetEncoding("windows-1252");
        }

        public ScriptParseResult Parse(FileContext context)
        {
            using StreamReader reader = new(context.FullFileName, this.win1251);

            ErrorSaver errorSaver = new();
            AntlrInputStream input = new(reader);
            CKLexer lexer = new(input);
            CommonTokenStream tokens = new(lexer);
            CKParser parser = new(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorSaver);

            StringsListener listener = new();
            listener.Folder = context.ModFolder;
            ParseTreeWalker walker = new();
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
            ErrorSaver errorSaver = new();
            TranslateListener listener;

            using (StreamReader reader = new(fileName, this.win1251))
            {
                AntlrInputStream input = new(reader);
                CKLexer lexer = new(input);
                CommonTokenStream tokens = new(lexer);
                CKParser parser = new(tokens);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(errorSaver);

                ParseTreeWalker walker = new();
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

        public ModuleInfo? ParseModule(string fileName)
        {
            using StreamReader reader = new(fileName, this.win1252);

            ErrorSaver errorSaver = new();
            AntlrInputStream input = new(reader);
            CKLexer lexer = new(input);
            CommonTokenStream tokens = new(lexer);
            CKParser parser = new(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorSaver);

            ModDescriptionListener listener = new();
            ParseTreeWalker walker = new();
            walker.Walk(listener, parser.script());

            string? name = listener.Strings.Find(s => s.Key.Path.LastStep == "name")?.Value;
            string? path = listener.Strings.Find(s => s.Key.Path.LastStep == "path")?.Value;
            string? archive = listener.Strings.Find(s => s.Key.Path.LastStep == "archive")?.Value;
            string[] dependencies = listener.Arrays.Find(a => a.Key.Path.LastStep == "dependencies")?.Value
                                    ?? Array.Empty<string>();

            if (name == null || path == null && archive == null)
            {
                return null;
            }

            return new ModuleInfo
            {
                Name = name,
                Path = path ?? archive,
                IsArchive = archive != null,
                Dependencies = dependencies
                                   ?.Select(name => new ModuleInfo { Name = name })
                                   .ToArray()
                               ?? Array.Empty<ModuleInfo>()
            };
        }
    }
}