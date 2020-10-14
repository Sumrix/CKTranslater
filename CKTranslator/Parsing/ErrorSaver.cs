using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKTranslator
{
    public class ErrorSaver : BaseErrorListener
    {
        public List<string> Messages = new List<string>();

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            IEnumerable<string> stack = ((Antlr4.Runtime.Parser)recognizer).GetRuleInvocationStack();
            stack = stack.Reverse();
            string stackString = string.Join(" ", stack);
            string info = string.Format("stack: [{0}]\n line {1}: {2} at {3}: {4}",
                stackString, line, charPositionInLine, offendingSymbol.ToString(), msg);
            this.Messages.Add(info);
        }
    }
}
