using System;
using System.IO;
using System.Linq;

namespace Core.Matching
{
    public class LetterComparer
    {
        private readonly float[,] similarities;

        private LetterComparer(int sourceCount, int targetCount)
        {
            this.similarities = new float[sourceCount, targetCount];
        }

        public float Compare(int letterIndex1, int letterIndex2)
        {
            return this.similarities[letterIndex1, letterIndex2];
        }

        public static LetterComparer Load(string fileName)
        {
            string[][] similarities = File.ReadAllLines(fileName)
                .Select(line => line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            int sourceCount = similarities.Length;
            int targetCount = similarities[0].Length;

            LetterComparer comparer = new(sourceCount, targetCount);

            for (int sourceIndex = 0; sourceIndex < sourceCount; sourceIndex++)
            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
            {
                int similarity = int.Parse(similarities[sourceIndex][targetIndex]);
                if (similarity > 0)
                {
                    similarity++;
                }

                comparer.similarities[sourceIndex, targetIndex] = similarity / 10f;
            }

            return comparer;
        }
    }
}