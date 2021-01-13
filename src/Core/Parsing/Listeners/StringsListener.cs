using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Core.Parsing.Listeners
{
    public class StringsListener : ScriptListener
    {
        protected override string CustomType(string text)
        {
            if (this.Folder == @"common\cultures" &&
                this.PathList.Count == 1 &&
                text != "alternate_start" &&
                text != "graphical_cultures")
            {
                IdManager.Cultures.Add(text);
                return IdManager.EncodeCulture(text);
            }

            if (IdManager.Cultures.Contains(text))
            {
                return IdManager.EncodeCulture(text);
            }

            return base.CustomType(text);
        }

        public override void ExitLvalue([NotNull] CKParser.LvalueContext context)
        {
            base.ExitLvalue(context);

            if (context.Start.Type == CKParser.STRING)
            {
                IdManager.AddValueToIgnore(context.GetText());
            }
        }

        protected override void ProcessString(ITerminalNode @string)
        {
            string text = @string.GetText().Trim().Trim('"');
            string lastStep = this.CurPath.LastStep.ToLower();
            if (IdManager.IgnoreValueRegex.IsMatch(text) ||
                !IdManager.StringKeys.Any(keyPattern => lastStep.EqualsWildcard(keyPattern)))
            {
                return;
            }

            base.ProcessString(@string);
        }
    }
}