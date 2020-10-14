using System;
using System.Collections.Generic;
using System.Linq;

namespace CKTranslator.Parsing
{
    public class Path : IEquatable<Path>
    {
        public List<string> Steps;
        public string LastStep
        {
            get
            {
                return this.Steps[this.Steps.Count - 1];
            }
        }
        public string LastTwoSteps
        {
            get
            {
                return (this.Steps.Count > 1 ? (this.Steps[this.Steps.Count - 2] + '.') : "") +
                    this.Steps[this.Steps.Count - 1];
            }
        }
        public string FirstStep
        {
            get
            {
                return this.Steps.First();
            }
        }

        public Path(List<string> steps)
        {
            this.Steps = steps;
        }

        public override string ToString()
        {
            return string.Join(".", this.Steps);
        }

        public bool Equals(Path other)
        {
            return this.Steps.Count == other.Steps.Count && this.Steps.SequenceEqual(other.Steps);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (string str in this.Steps)
                {
                    hash = hash * 31 + str.GetHashCode();
                }
                return hash;
            }
        }

        public void AddForward(string step)
        {
            this.Steps.Insert(0, step);
        }

        public static Path Parse(string text)
        {
            return new Path(
                text.Split('.')
                    .ToList()
            );
        }
    }
}
