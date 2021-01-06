using System;

namespace Core.Parsing
{
    public class ScriptKey : IEquatable<ScriptKey>
    {
        public Path Path { get; set; }
        public int RepetitionIndex { get; set; }

        public bool Equals(ScriptKey other)
        {
            return this.Path.Equals(other.Path); // && this.RepetitionIndex.Equals(other.RepetitionIndex);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode(); // ^ this.RepetitionIndex;
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", this.RepetitionIndex, this.Path);
        }
    }
}