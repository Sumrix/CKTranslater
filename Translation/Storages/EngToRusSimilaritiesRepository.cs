namespace Translation.Storages
{
    public class EngToRusSimilaritiesRepository : Repository
    {
        private Matrix data;

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

        protected override void LoadData(string fileName)
        {
            this.data = JsonHelper.Deserialize<Matrix>(fileName);
        }

        protected override object GetDataToSave()
        {
            return this.data;
        }

        private class Matrix
        {
            // Если сделать readonly, не будет работать десериализация
            public float[,] Items;

            public Matrix()
            {
                // +1 для пустого символа
                this.Items = new float[DB.EngLetters.Count + 1, DB.RusLetters.Count + 1];
            }
        }
    }
}