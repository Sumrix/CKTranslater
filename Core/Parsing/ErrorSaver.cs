using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;

namespace Core
{
    public class ErrorSaver : BaseErrorListener
    {
        public List<string> Messages = new();

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine,
            string msg, RecognitionException e)
        {
            IEnumerable<string> stack = ((Parser) recognizer).GetRuleInvocationStack();
            stack = stack.Reverse();
            string stackString = string.Join(" ", stack);
            string info = string.Format("stack: [{0}]\n line {1}: {2} at {3}: {4}",
                stackString, line, charPositionInLine, offendingSymbol, msg);
            this.Messages.Add(info);
        }
    }
}