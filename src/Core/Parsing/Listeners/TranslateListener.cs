using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Core.Parsing.Listeners
{
    public class TranslateListener : ScriptListener
    {
        public readonly TokenStreamRewriter Rewriter;
        private readonly StringTranslateHandle translator;

        public TranslateListener(ITokenStream tokens, StringTranslateHandle translator)
        {
            this.Rewriter = new TokenStreamRewriter(tokens);
            this.translator = translator;
        }

        protected override void ProcessString(ITerminalNode @string)
        {
            base.ProcessString(@string);

            var name2 = new HashSet<string>
            {
                "add_character_modifier.name",
                "character_event.name",
                "create_character.name",
                "defined_text.name",
                "has_game_rule.name",
                "option.name",
                "per_attribute.name",
                "add_province_modifier.name"
            };

            ScriptString s = this.Strings.Last();
            if (IdManager.IgnoreValues.Contains(s.Value) ||
                IdManager.IgnoreValueRegex.IsMatch(s.Value) ||
                !IdManager.StringKeys.Any(keyPattern => s.Key.Path.LastStep.EqualsWildcard(keyPattern)) ||
                name2.Contains(s.Key.Path.LastTwoSteps))
            {
                return;
            }

            string translation = this.translator(s);

            if (translation != null)
            {
                string str = @string.GetText();
                string begin;

                begin = str[0] == '\'' ? "\'" : "\"";

                string end = "";

                int i = str.Length - 1;
                while (i >= 0 && char.IsWhiteSpace(str[i]))
                {
                    end = str[i] + end;
                    i--;
                }

                if (str[i] == '\'')
                {
                    end = '\'' + end;
                }
                else
                {
                    end = '"' + end;
                }

                translation = begin + translation + end;

                this.Rewriter.Replace(@string.Symbol, translation);
            }
        }
    }
}