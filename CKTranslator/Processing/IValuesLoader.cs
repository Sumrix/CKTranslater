using CKTranslator.Parsing;
using System.Collections.Generic;

namespace CKTranslator.Processing
{
    public enum Language
    {
        Rus,
        Eng
    }

    public interface IValuesLoader
    {
        Language Language { set; }
        Dictionary<ScriptKey, string> Strings { get; set; }
        Dictionary<ScriptKey, string[]> Arrays { get; }

        Event Load(FileContext context);
    }
}
