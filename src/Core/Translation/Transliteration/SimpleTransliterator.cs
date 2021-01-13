using System.IO;
using System.Linq;

namespace Core.Translation.Transliteration
{
    /// <summary>
    ///     Элементарный переводчик
    /// </summary>
    public class SimpleTransliterator
    {
        private readonly string[] map;

        public SimpleTransliterator(string[] map)
        {
            this.map = map;
        }

        public static SimpleTransliterator Load(string fileName)
        {
            var mapRules = File.ReadAllLines(fileName)
                .Select(line => line.Split('\t'))
                .ToList();
            int maxChar = mapRules.Max(p => p[0][0]);
            string[] map = new string[maxChar + 1];

            foreach (string[] rule in mapRules)
            {
                map[rule[0][0]] = rule[1];
            }

            return new SimpleTransliterator(map);
        }

        public string Translate(string word)
        {
            return string.Concat(
                word.Select(c => this.map[c])
            );
        }

        public string Translate(char letter)
        {
            return this.map[letter];
        }
    }
}