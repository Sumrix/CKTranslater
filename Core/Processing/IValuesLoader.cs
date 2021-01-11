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
        // TODO: поменять на IReadonlyDictionary
        IDictionary<ScriptKey, string[]> Arrays { get; }
        Language Language { set; }
        IDictionary<ScriptKey, string> Strings { get; set; }
        Event? Load(FileContext context);
    }
}