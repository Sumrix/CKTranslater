namespace Core.Transliteration
{
    /// <summary>
    ///     Узел графа по которому производится перевод
    /// </summary>
    public class GraphemeVariant
    {
        public string[] Options;
        public GraphemeVariant[] Variants;
    }
}