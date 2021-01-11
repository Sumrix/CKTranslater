using System;

namespace Core.Translation.Transliteration
{
    /// <summary>
    ///     Узел графа по которому производится перевод
    /// </summary>
    public class GraphemeVariant
    {
        public string?[] Options = Array.Empty<string?>();
        public GraphemeVariant?[] Variants = Array.Empty<GraphemeVariant?>();
    }
}