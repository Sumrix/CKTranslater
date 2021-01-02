using System;
using System.Linq;
using Translation.Collections;

namespace Translation.Graphemes
{
    public enum GraphemeType
    {
        Silent,
        Mixed,
        Vowel,
        Consonant
    }

    [Flags]
    public enum CommonFlag : uint
    {
        First = 1 << 0,
        Last = 1 << 1
    }

    [Flags]
    public enum VowelFlag : uint
    {
        First = CommonFlag.First,
        Last = CommonFlag.Last,
        OpenSyllable = 1 << 2,
        PreviousVowel = 1 << 3,
        NextVowel = 1 << 4,
        Stressed = 1 << 5
    }

    [Flags]
    public enum ConsonantFlag : uint
    {
        First = CommonFlag.First,
        Last = CommonFlag.Last,
        NextVowel = 1 << 2,
        PreviousVowel = 1 << 3
    }

    public class Grapheme
    {
        public static readonly uint[] MaxFlag = new uint[4]
        {
            0, (uint) CommonFlag.Last, (uint) VowelFlag.Stressed, (uint) ConsonantFlag.PreviousVowel
        };

        public static readonly int[] FlagVariants = MaxFlag
            .Select(f => f == 0 ? 1 : (int) (f << 1))
            .ToArray();

        public static readonly int[] FlagBitNum = MaxFlag
            .Select(f => f == 0 ? 0 : Bit.OnesCount(f - 1) + 1)
            .ToArray();

        public FlagSet Flags;

        public Grapheme(GraphemeType type, string letters, FlagSet flags = default)
        {
            this.Type = type;
            this.Letters = letters;
            this.Flags = flags;
        }

        public GraphemeType Type { get; private set; }
        public string Letters { get; private set; }

        public Grapheme Clone()
        {
            return new Grapheme(this.Type, this.Letters, this.Flags);
        }

        //public Grapheme MergeWith(Grapheme other) => this.Type switch
        //{
        //    GraphemeType.Silent => new Grapheme(other.Type, this.Letters + other.Letters, other.Flags),
        //    GraphemeType.Mixed => new Grapheme(GraphemeType.Mixed, this.Letters + other.Letters, 0u),
        //    GraphemeType.Vowel => other.Type switch
        //    {
        //        GraphemeType.Silent => new Grapheme(GraphemeType.Vowel, this.Letters + other.Letters, this.Flags),
        //        GraphemeType.Mixed => new Grapheme(GraphemeType.Mixed, this.Letters + other.Letters, 0u),
        //        GraphemeType.Vowel => new Grapheme(GraphemeType.Vowel, this.Letters + other.Letters, 
        //            (uint)MergeFlags((VowelFlag)this.Flags, (VowelFlag)other.Flags)),
        //        GraphemeType.Consonant => new Grapheme(GraphemeType.Mixed, this.Letters + other.Letters, 0u),
        //        _ => throw new Exception("Неверный тип")
        //    },
        //    GraphemeType.Consonant => other.Type switch
        //    {
        //        GraphemeType.Silent => new Grapheme(GraphemeType.Consonant, this.Letters + other.Letters, this.Flags),
        //        GraphemeType.Mixed => new Grapheme(GraphemeType.Mixed, this.Letters + other.Letters, 0u),
        //        GraphemeType.Vowel => new Grapheme(GraphemeType.Mixed, this.Letters + other.Letters, 0u),
        //        GraphemeType.Consonant => new Grapheme(GraphemeType.Consonant, this.Letters + other.Letters, 
        //            (uint)MergeFlags((ConsonantFlag)this.Flags, (ConsonantFlag)other.Flags)),
        //        _ => throw new Exception("Неверный тип")
        //    },
        //    _ => throw new Exception("Неверный тип")
        //};

        public Grapheme MergeWith(Grapheme other)
        {
            this.Letters += other.Letters;

            switch (this.Type)
            {
                case GraphemeType.Silent:
                    this.Type = other.Type;
                    this.Flags = other.Flags;
                    break;
                case GraphemeType.Vowel:
                    switch (other.Type)
                    {
                        case GraphemeType.Mixed:
                            this.Type = GraphemeType.Mixed;
                            this.Flags = new FlagSet((uint) MergeFlags((CommonFlag) this.Flags.Value,
                                (CommonFlag) other.Flags.Value));
                            break;
                        case GraphemeType.Vowel:
                            this.Flags = new FlagSet((uint) MergeFlags((VowelFlag) this.Flags.Value,
                                (VowelFlag) other.Flags.Value));
                            break;
                        case GraphemeType.Consonant:
                            this.Type = GraphemeType.Mixed;
                            this.Flags = new FlagSet(0);
                            break;
                    }

                    break;
                case GraphemeType.Consonant:
                    switch (other.Type)
                    {
                        case GraphemeType.Mixed:
                            this.Type = GraphemeType.Mixed;
                            this.Flags = new FlagSet((uint) MergeFlags((CommonFlag) this.Flags.Value,
                                (CommonFlag) other.Flags.Value));
                            break;
                        case GraphemeType.Vowel:
                            this.Type = GraphemeType.Mixed;
                            this.Flags = new FlagSet(0);
                            break;
                        case GraphemeType.Consonant:
                            this.Flags = new FlagSet((uint) MergeFlags((ConsonantFlag) this.Flags.Value,
                                (ConsonantFlag) other.Flags.Value));
                            break;
                    }

                    break;
                case GraphemeType.Mixed:
                    this.Flags =
                        new FlagSet((uint) MergeFlags((CommonFlag) this.Flags.Value, (CommonFlag) other.Flags.Value));
                    break;
            }

            return this;
        }

        private static VowelFlag MergeFlags(VowelFlag left, VowelFlag right)
        {
            return (left & (VowelFlag.First | VowelFlag.PreviousVowel | VowelFlag.Stressed))
                   | (right & (VowelFlag.Last | VowelFlag.OpenSyllable | VowelFlag.NextVowel | VowelFlag.Stressed));
        }

        private static ConsonantFlag MergeFlags(ConsonantFlag left, ConsonantFlag right)
        {
            return (left & (ConsonantFlag.First | ConsonantFlag.PreviousVowel))
                   | (right & (ConsonantFlag.Last | ConsonantFlag.NextVowel));
        }

        private static CommonFlag MergeFlags(CommonFlag left, CommonFlag right)
        {
            return (CommonFlag) (((uint) left & 3) | ((uint) right & 3));
        }

        public override string ToString()
        {
            return $"{this.Type} \"{this.Letters}\" [{Bit.ToString(this.Flags.Value, FlagBitNum[(int) this.Type])}]";
        }
    }
}