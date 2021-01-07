//using MoreLinq.Extensions;

using System;
using System.Linq;

namespace Core.Matching
{
    public class FVector
    {
        public readonly int Length;
        public readonly string Letters0;
        public readonly string Letters1;
        public readonly int NullLetterCount;
        private readonly int Number;
        public readonly float PathAverage;
        public readonly FVector Previous;
        private readonly int previousLenghts;
        private readonly float previousSums;
        public readonly float Sum;

        private FVector(FVector prevous,
            float sum,
            int length,
            float previousSums,
            int previousLenghts,
            int number,
            string letters0,
            string letters1,
            float pathAverage = -1)
        {
            this.Previous = prevous;
            this.Sum = sum;
            this.Length = length;
            this.previousSums = previousSums;
            this.previousLenghts = previousLenghts;
            this.Number = number;
            this.Letters0 = letters0;
            this.Letters1 = letters1;
            this.NullLetterCount = (prevous?.NullLetterCount ?? 0) + (
                (string.IsNullOrEmpty(letters0) ? letters1?.Length :
                    string.IsNullOrEmpty(letters1) ? letters0?.Length : 0) ?? 0);
            this.PathAverage = pathAverage == -1
                ? (this.previousSums + this.Sum) / (this.previousLenghts + this.Length)
                : pathAverage;
        }

        public float Average => this.Sum / this.Length;

        public static FVector Continue(FVector p, float value, char? letter0, char? letter1)
        {
            return new(
                p.Previous,
                p.Sum == 0 || value == 0 && !(letter0 == null && letter1 == null) ? 0 : p.Sum + value,
                p.Length + (letter0 == null && letter1 == null ? 0 : 1),
                p.previousSums,
                p.previousLenghts,
                p.Number,
                p.Letters0 + letter0,
                p.Letters1 + letter1
            );
        }

        public FVector Continue(float value, char? letter0, char? letter1)
        {
            return FVector.Continue(this, value, letter0, letter1);
        }

        public static FVector Max(FVector first, FVector second)
        {
            if (second == null)
            {
                return first;
            }

            if (first == null)
            {
                return second;
            }

            if (Math.Abs(first.PathAverage - second.PathAverage) < 0.001f)
            {
                if (first.NullLetterCount < second.NullLetterCount)
                {
                    return first;
                }

                return second;
            }

            if (first.PathAverage > second.PathAverage)
            {
                return first;
            }

            return second;
        }

        public static FVector Max(params FVector[] vectors)
        {
            return vectors.Aggregate(FVector.Max);
        }

        public static FVector New(FVector p, float value, char? letter0, char? letter1)
        {
            return new(
                p,
                value,
                letter0 == null && letter1 == null ? 0 : 1,
                p.previousSums + p.Sum,
                p.previousLenghts + p.Length,
                p.Number + 1,
                letter0?.ToString(),
                letter1?.ToString()
            );
        }

        public FVector New(float value, char? letter0, char? letter1)
        {
            return FVector.New(this, value, letter0, letter1);
        }

        public static FVector Start(float value, char? letter0, char? letter1)
        {
            return new(
                null,
                value,
                1,
                0,
                0,
                0,
                letter0?.ToString(),
                letter1?.ToString(),
                value
            );
        }

        public override string ToString()
        {
            return this == null
                ? "null"
                : $"{this?.Letters0}; {this?.Letters1}; {this?.PathAverage}";
        }
    }
}