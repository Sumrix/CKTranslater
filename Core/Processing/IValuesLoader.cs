using System.Collections.Generic;
using Core.Parsing;

namespace Core.Processing
{
    public enum Language
    {
        Rus,
        Eng
    }

    public interface IValuesLoader
    {
        Dictionary<ScriptKey, string[]> Arrays { get; }
        Language Language { set; }
        Dictionary<ScriptKey, string> Strings { get; set; }
        Event Load(FileContext context);
    }
}