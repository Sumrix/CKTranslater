using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;

namespace Core.Parsing
{
    public class ErrorSaver : BaseErrorListener
    {
        public readonly List<string> Messages = new();

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine,
            string msg, RecognitionException e)
        {
            IEnumerable<string> stack = ((Parser) recognizer).GetRuleInvocationStack();
            stack = stack.Reverse();
            string stackString = string.Join(" ", stack);
            string info = $"stack: [{stackString}]\n line {line}: {charPositionInLine} at {offendingSymbol}: {msg}";
            this.Messages.Add(info);
        }
    }
}