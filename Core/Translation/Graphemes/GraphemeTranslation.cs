using System.Collections.Generic;
using Core.Matching;

namespace Core.Graphemes
{
    public class GraphemeTranslation
    {
        public Grapheme Original;
        public string Translation;

        public GraphemeTranslation()
        {
            this.Original = new Grapheme(GraphemeType.Silent, "");
        }

        public GraphemeTranslation(Grapheme original, string translation)
        {
            this.Original = original;
            this.Translation = translation;
        }

        public GraphemeTranslation Clone()
        {
            return new(this.Original.Clone(), this.Translation);
        }

        public static List<GraphemeTranslation> Create(IReadOnlyCollection<LettersMatch> matches,
            IEnumerable<Grapheme> graphemes)
        {
            var translations = new List<GraphemeTranslation>(matches.Count);
            var ge = graphemes.GetEnumerator();
            ge.MoveNext();

            foreach (LettersMatch match in matches)
            {
                Grapheme g = new(GraphemeType.Silent, "");

                for (int i = 0; i < match.Letters0.Length; i++)
                {
                    g.MergeWith(ge.Current);
                    ge.MoveNext();
                }

                translations.Add(new GraphemeTranslation(g, match.Letters1));
            }

            return translations;
        }

        public GraphemeTranslation MergeWith(GraphemeTranslation other)
        {
            this.Original.MergeWith(other.Original);
            this.Translation += other.Translation;

            return this;
        }

        public override string ToString()
        {
            return $"{this.Original}; {this.Translation}";
        }
    }
}