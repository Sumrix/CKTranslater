//using MoreLinq.Extensions;
using System;
using System.Linq;

namespace Translation.Matching
{
    public class FVector
    {
        public readonly FVector Previous;
        public readonly float Sum;
        public readonly int Length;
        private readonly float previousSums;
        private readonly int previousLenghts;
        private readonly int Number;
        public readonly float PathAverage;
        public float Average => this.Sum / this.Length;
        public readonly string Letters0;
        public readonly string Letters1;
        public readonly int NullLetterCount;

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

        public static FVector Start(float value, char? letter0, char? letter1)
        => new FVector
        (
            prevous: null,
            sum: value,
            length: 1,
            previousSums: 0,
            previousLenghts: 0,
            number: 0,
            letters0: letter0?.ToString(),
            letters1: letter1?.ToString(),
            pathAverage: value
        );

        public static FVector New(FVector p, float value, char? letter0, char? letter1) 
        => new FVector
        (
            prevous: p,
            sum: value,
            length: letter0 == null && letter1 == null ? 0 : 1,
            previousSums: p.previousSums + p.Sum,
            previousLenghts: p.previousLenghts + p.Length,
            number: p.Number + 1,
            letters0: letter0?.ToString(),
            letters1: letter1?.ToString()
        );

        public static FVector Continue(FVector p, float value, char? letter0, char? letter1) 
        => new FVector
        (
            prevous: p.Previous,
            sum: p.Sum == 0 || value == 0 && !(letter0 == null && letter1 == null) ? 0 : p.Sum + value,
            length: p.Length + (letter0 == null && letter1 == null ? 0 : 1),
            previousSums: p.previousSums,
            previousLenghts: p.previousLenghts,
            number: p.Number,
            letters0: p.Letters0 + letter0?.ToString(),
            letters1: p.Letters1 + letter1?.ToString()
        );

        public static FVector Max(FVector first, FVector second)
        {
            if (second == null)
            {
                 return first;
            }
            else if (first == null)
            {
                return second;
            }
            else if (Math.Abs(first.PathAverage - second.PathAverage) < 0.001f)
            {
                if (first.NullLetterCount < second.NullLetterCount)
                {
                    return first;
                }
                else
                {
                    return second;
                }
            }
            else if (first.PathAverage > second.PathAverage)
            {
                return first;
            }
            else
            {
                return second;
            }
        }

        public static FVector Max(params FVector[] vectors) => 
            vectors.Aggregate(FVector.Max);

        public FVector Continue(float value, char? letter0, char? letter1) =>
            FVector.Continue(this, value, letter0, letter1);

        public FVector New(float value, char? letter0, char? letter1) =>
            FVector.New(this, value, letter0, letter1);

        public override string ToString()
        {
            return this == null
                ? "null"
                : $"{this?.Letters0}; {this?.Letters1}; {this?.PathAverage}";
        }
    }
}
