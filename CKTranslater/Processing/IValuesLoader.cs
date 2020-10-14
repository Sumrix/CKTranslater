using CKTranslater.Parsing;
using System.Collections.Generic;

namespace CKTranslater.Processing
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
