using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Parsing
{
    public class Path : IEquatable<Path>
    {
        public readonly List<string> Steps;

        public Path(List<string> steps)
        {
            this.Steps = steps;
        }

        public string FirstStep => this.Steps.First();
        public string LastStep => this.Steps[^1];

        public string LastTwoSteps =>
            (this.Steps.Count > 1 ? this.Steps[^2] + '.' : "") +
            this.Steps[^1];

        public bool Equals(Path? other)
        {
            return other != null && this.Steps.Count == other.Steps.Count && this.Steps.SequenceEqual(other.Steps);
        }

        public void AddForward(string step)
        {
            this.Steps.Insert(0, step);
        }

        public override bool Equals(object? obj)
        {
            return obj is Path path && this.Equals(path);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Steps.Aggregate(19, (current, str) => current * 31 + str.GetHashCode());
            }
        }

        public static Path Parse(string text)
        {
            return new(
                text.Split('.')
                    .ToList()
            );
        }

        public override string ToString()
        {
            return string.Join(".", this.Steps);
        }
    }
}