namespace Translation.Storages
{
    public class EngToRusSimilaritiesDB : BaseDB<EngToRusSimilaritiesDB.Matrix>
    {
        public class Matrix
        {
            public float[,] Items;
            
            public Matrix()
            {
                // +1 для пустого символа
                this.Items = new float[DB.EngLetters.Count + 1, DB.RusLetters.Count + 1];
            }
        }

        public int EngCount => this.data.Items.GetLength(0);
        public int RusCount => this.data.Items.GetLength(1);

        public float this[int engletterIndex, int rusletterIndex]
        {
            get => this.data.Items[engletterIndex, rusletterIndex];
            set => this.data.Items[engletterIndex, rusletterIndex] = value;
        }

        public float EmptyRusToEng(int engLetterIndex)
        {
            return this.data.Items[engLetterIndex, this.RusCount - 1];
        }

        public float EmptyEngToRus(int rusLetterIndex)
        {
            return this.data.Items[this.EngCount - 1, rusLetterIndex];
        }

        public static EngToRusSimilaritiesDB Load(string fileName = null)
        {
            return EngToRusSimilaritiesDB.LoadFromFile<EngToRusSimilaritiesDB>(fileName ?? FileName.EngToRusSimilarities);
        }
    }
}
