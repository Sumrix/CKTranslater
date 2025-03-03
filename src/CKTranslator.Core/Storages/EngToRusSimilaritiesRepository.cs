﻿namespace CKTranslator.Core.Storages
{
    public class EngToRusSimilaritiesRepository : Repository
    {
        // +1 для пустого символа
        private float[,] data = new float[Db.EngLetters.Count + 1, Db.RusLetters.Count + 1];

        public int EngCount => this.data.GetLength(0);

        public int RusCount => this.data.GetLength(1);

        public float this[int engletterIndex, int rusletterIndex]
        {
            get => this.data[engletterIndex, rusletterIndex];
            set => this.data[engletterIndex, rusletterIndex] = value;
        }

        public float EmptyEngToRus(int rusLetterIndex)
        {
            return this.data[this.EngCount - 1, rusLetterIndex];
        }

        public float EmptyRusToEng(int engLetterIndex)
        {
            return this.data[engLetterIndex, this.RusCount - 1];
        }

        protected override object GetDataToSave()
        {
            return this.data;
        }

        protected override void LoadData(string fileName)
        {
            this.data = JsonHelper.Deserialize<float[,]>(fileName);
        }
    }
}