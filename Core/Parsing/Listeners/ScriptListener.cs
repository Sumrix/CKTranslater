using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Core.Parsing.Listeners
{
    public class ScriptListener : CKBaseListener
    {
        private static readonly string[] allowedArrays = { "female_names", "male_names", "dependencies" };
        public readonly List<ScriptArray> Arrays = new();
        protected readonly LinkedList<string> PathList = new();
        private readonly Dictionary<Path, int> repetitions = new();
        public readonly List<ScriptString> Strings = new();
        protected Path CurPath = new(new List<string>());
        public string Folder = string.Empty;
        private int repetitionIndex;

        protected virtual string CustomType(string text)
        {
            return text;
        }

        public override void ExitLvalue([NotNull] CKParser.LvalueContext context)
        {
            string text = context.GetText();

            //string lvalueText = context.Start.Type switch
            //{
            //    CKParser.INT => "INT",
            //    CKParser.DATE => "DATE",
            //    CKParser.BOOL => "BOOL",
            //    CKParser.FLOAT => "FLOAT",
            //    CKParser.HEX => "HEX",
            //    _ => "cdbek".Contains(text[0]) && text.Length > 1 && text[1] == '_' ? "TITLE" : this.CustomType(text)
            //};

            this.PathList.AddLast(this.CustomType(text));
        }

        public override void ExitRecord([NotNull] CKParser.RecordContext context)
        {
            this.CurPath = new Path(this.PathList.ToList());

            CKParser.RvalueContext rvalue = context.rvalue();
            if (rvalue != null)
            {
                this.ProcessRValue(rvalue);
            }

            this.PathList.RemoveLast();
        }

        protected virtual void ProcessArray(ITerminalNode[] stringArray)
        {
            this.Arrays.Add(
                new ScriptArray
                {
                    Key = new ScriptKey
                    {
                        Path = this.CurPath,
                        RepetitionIndex = this.repetitionIndex
                    },
                    Value = stringArray.Select(x => x.GetText().Trim().Trim('"')).ToArray()
                }
            );
        }

        private void ProcessRValue(CKParser.RvalueContext rvalue)
        {
            ITerminalNode @string = rvalue.STRING();
            if (@string != null)
            {
                this.UpdateRepetitionIndex();
                this.ProcessString(@string);
                return;
            }

            var stringArray = rvalue.block()?.array()?.STRING();
            if (stringArray != null && ScriptListener.allowedArrays.Contains(this.PathList.Last?.Value))
            {
                this.UpdateRepetitionIndex();
                this.ProcessArray(stringArray);
            }
        }

        protected virtual void ProcessString(ITerminalNode @string)
        {
            string text = @string.GetText().Trim().Trim('"');

            this.Strings.Add(
                new ScriptString
                {
                    Key = new ScriptKey
                    {
                        Path = this.CurPath,
                        RepetitionIndex = this.repetitionIndex
                    },
                    Value = text
                }
            );
        }

        private void UpdateRepetitionIndex()
        {
            if (this.repetitions.TryGetValue(this.CurPath, out this.repetitionIndex))
            {
                this.repetitionIndex++;
            }
            else
            {
                this.repetitionIndex = 0;
            }

            this.repetitions[this.CurPath] = this.repetitionIndex;
        }
    }
}