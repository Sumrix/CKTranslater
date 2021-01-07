using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Core.Parsing.Listeners
{
    public class ScriptListener : CKBaseListener
    {
        private static readonly string[] allowedArrays = { "female_names", "male_names", "dependencies" };
        protected readonly LinkedList<string> pathList = new();
        private readonly Dictionary<Path, int> repetitions = new();
        public List<ScriptArray> Arrays = new();
        protected Path curPath = new(new List<string>());
        public string Folder;
        private int repetitionIndex;
        public List<ScriptString> Strings = new();

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

            this.pathList.AddLast(this.CustomType(text));
        }

        public override void ExitRecord([NotNull] CKParser.RecordContext context)
        {
            this.curPath = new Path(this.pathList.ToList());

            CKParser.RvalueContext rvalue = context.rvalue();
            if (rvalue != null)
            {
                this.ProcessRValue(rvalue);
            }

            this.pathList.RemoveLast();
        }

        protected virtual void ProcessArray(ITerminalNode[] stringArray)
        {
            this.Arrays.Add(
                new ScriptArray
                {
                    Key = new ScriptKey
                    {
                        Path = this.curPath,
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
            if (stringArray != null && ScriptListener.allowedArrays.Contains(this.pathList.Last.Value))
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
                        Path = this.curPath,
                        RepetitionIndex = this.repetitionIndex
                    },
                    Value = text
                }
            );
        }

        private void UpdateRepetitionIndex()
        {
            if (this.repetitions.TryGetValue(this.curPath, out this.repetitionIndex))
            {
                this.repetitionIndex++;
            }
            else
            {
                this.repetitionIndex = 0;
            }

            this.repetitions[this.curPath] = this.repetitionIndex;
        }
    }
}