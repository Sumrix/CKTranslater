using System;

namespace CKTranslator.Core.Parsing
{
    public class ScriptKey : IEquatable<ScriptKey>
    {
        public Path Path { get; init; }
        public int RepetitionIndex { get; init; }

        public bool Equals(ScriptKey? other)
        {
            return
                other != null && this.Path.Equals(other.Path); // && this.RepetitionIndex.Equals(other.RepetitionIndex);
        }

        public override bool Equals(object? obj)
        {
            return obj is ScriptArray other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode(); // ^ this.RepetitionIndex;
        }

        public override string ToString()
        {
            return $"[{this.RepetitionIndex}]{this.Path}";
        }
    }
}