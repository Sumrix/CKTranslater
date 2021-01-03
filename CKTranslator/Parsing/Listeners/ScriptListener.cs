using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CKTranslator.Parsing.Listeners
{
    public class ScriptListener : CKBaseListener
    {
        public List<ScriptString> Strings = new List<ScriptString>();
        public List<ScriptArray> Arrays = new List<ScriptArray>();
        protected readonly LinkedList<string> pathList = new LinkedList<string>();
        protected Path curPath = new Path(new List<string>());
        private static readonly string[] allowedArrays = { "female_names", "male_names", "dependencies" };
        private int repetitionIndex = 0;
        private readonly Dictionary<Path, int> repetitions = new Dictionary<Path, int>();
        public string Folder;

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

        protected virtual string CustomType(string text)
        {
            return text;
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

        private void ProcessRValue(CKParser.RvalueContext rvalue)
        {
            ITerminalNode @string = rvalue.STRING();
            if (@string != null)
            {
                this.UpdateRepetitionIndex();
                this.ProcessString(@string);
                return;
            }

            ITerminalNode[] stringArray = rvalue.block()?.array()?.STRING();
            if (stringArray != null && allowedArrays.Contains(this.pathList.Last.Value))
            {
                this.UpdateRepetitionIndex();
                this.ProcessArray(stringArray);
                return;
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
    }
}
