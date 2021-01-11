using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Translation.Interpolation
{
    public static class Interpolater
    {
        public static bool Debug = false;

        private static List<uint> GetTerms(IReadOnlyList<string?> vector, params string?[] values)
        {
            var positions = new List<uint>();
            for (uint p = 0; p < vector.Count; p++)
            {
                if (values.Contains(vector[(int) p]))
                {
                    positions.Add(p);
                }
            }

            return positions;
        }

        public static void Interpolate(string?[] vector, int numBits)
        {
            string?[] types = vector.Distinct().Where(c => !string.IsNullOrEmpty(c)).ToArray();
            DontCare?[] dcs = new DontCare[1 << numBits];

            foreach (string? type in types)
            {
                int typeRank = vector.Count(t => t == type);
                var terms = Interpolater.GetTerms(vector, type, null);
                var essentialTerms = Interpolater.GetTerms(vector, type);
                var prime = Simplifier.GetPrimeImplicants(terms, numBits);
                var essentialImpls = Simplifier.GetEssentialImplicants(essentialTerms, prime, numBits);
                var reducedImpls = Simplifier.ReduceImplicants(essentialImpls.ToArray(), essentialTerms, numBits);

                foreach (uint impl in reducedImpls)
                {
                    uint[] perms = Simplifier.Permutations(impl, numBits).ToArray();

                    if (Interpolater.Debug)
                    {
                        Console.Write(type + ": " + string.Concat(Interpolater.ToVector(perms, vector.Length, type)) +
                                      " " + Bit.ToString(impl, numBits));
                        Console.WriteLine();
                    }

                    foreach (uint perm in perms)
                    {
                        DontCare dc = dcs[perm] ?? new DontCare();

                        int implRank = Bit.OnesCount(impl >> numBits);

                        if (dc.ImplRank < implRank ||
                            dc.ImplRank == implRank && dc.TypeRank < typeRank)
                        {
                            dc.Type = type;
                            dc.TypeRank = typeRank;
                            dc.ImplRank = implRank;
                        }

                        dcs[perm] = dc;
                    }
                }
            }

            for (int term = 0; term < vector.Length; term++)
            {
                string? type = vector[term];
                if (type == null)
                {
                    vector[term] = dcs[term]?.Type;
                }

                vector[term] ??= types.Length == 0 ? "" : types[0];
            }
        }

        private static string?[] ToVector(IReadOnlyList<uint> terms, int length, string? value)
        {
            string?[] vector = new string?[length];

            for (int i = 0, ti = 0; i < length; i++)
            {
                if (ti < terms.Count && i == terms[ti])
                {
                    vector[i] = value;
                    ti++;
                }
                else
                {
                    vector[i] = "-";
                }
            }

            return vector;
        }

        private class DontCare
        {
            public int ImplRank;
            public string? Type;
            public int TypeRank;
        }
    }
}