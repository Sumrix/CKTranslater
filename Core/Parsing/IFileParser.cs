using Core.Processing;

namespace Core.Parsing
{
    public delegate string StringTranslateHandle(ScriptString @string);

    public interface IFileParser
    {
        ScriptParseResult Parse(FileContext context);
        ScriptParseResult Translate(string fileName, StringTranslateHandle translator);
    }
}