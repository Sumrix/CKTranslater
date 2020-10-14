using System.Collections.Generic;

using Translation.Matching;

namespace Translation.Graphemes
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

        public GraphemeTranslation MergeWith(GraphemeTranslation other)
        {
            this.Original.MergeWith(other.Original);
            this.Translation += other.Translation;

            return this;
        }

        public GraphemeTranslation Clone()
        {
            return new GraphemeTranslation(this.Original.Clone(), this.Translation);
        }

        public static List<GraphemeTranslation> Convert(IReadOnlyCollection<LettersMatch> matches, IEnumerable<Grapheme> graphemes)
        {
            List<GraphemeTranslation> translations = new List<GraphemeTranslation>(matches.Count);
            IEnumerator<Grapheme> ge = graphemes.GetEnumerator();
            ge.MoveNext();

            foreach (LettersMatch match in matches)
            {
                Grapheme g = new Grapheme(GraphemeType.Silent, "");

                for (int i = 0; i < match.Letters0.Length; i++)
                {
                    g.MergeWith(ge.Current);
                    ge.MoveNext();
                }

                translations.Add(new GraphemeTranslation(g, match.Letters1));
            }

            return translations;
        }

        public override string ToString()
        {
            return $"{this.Original}; {this.Translation}";
        }
    }
}
