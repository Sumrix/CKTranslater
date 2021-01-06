namespace NameTranslation.Matching
{
    public class LettersMatch
    {
        public readonly string Letters0;
        public readonly string Letters1;
        public readonly float Similarity;

        public LettersMatch(string letters0, string letters1, float similarity)
        {
            this.Letters0 = letters0;
            this.Letters1 = letters1;
            this.Similarity = similarity;
        }

        public override string ToString()
        {
            return $"{this.Letters0};{this.Letters1};{this.Similarity}";
        }
    }
}