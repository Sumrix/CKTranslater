using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using CKTranslator.Core.Models;
using CKTranslator.Core.Parsing.Listeners;

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CKTranslator.Core.Parsing
{
    public class ScriptParser
    {
        private readonly Encoding win1251;
        private readonly Encoding win1252;

        public ScriptParser()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.win1251 = Encoding.GetEncoding("windows-1251");
            this.win1252 = Encoding.GetEncoding("windows-1252");
        }

        public Module? ParseModule(string fileName)
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

            return new Module
            {
                Name = name,
                Path = path ?? archive,
                IsArchive = archive != null,
                Dependencies = dependencies
                                   ?.Select(name => new Module { Name = name })
                                   .ToArray()
                               ?? Array.Empty<Module>()
            };
        }

        public ScriptParseResult Translate(string fileName, Func<string, string> translator)
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
    }
}