using System;

namespace CKTranslator.Core.Storages
{
    /// <summary>
    ///     Базовый класс для объектов хранилищ данных
    /// </summary>
    public abstract class Repository
    {
        private string? fileName;

        public static TRepository Load<TRepository>(string fileName)
            where TRepository : Repository, new()
        {
            TRepository db = new();
            db.Load(fileName);
            return db;
        }

        public void Load(string fileName)
        {
            this.fileName = fileName;
            this.LoadData(fileName);
        }

        public void Save(string? fileName = null)
        {
            string fileNameToSave = fileName ?? this.fileName ?? throw new ArgumentNullException(nameof(fileName));
            JsonHelper.Serialize(this.GetDataToSave(), fileNameToSave);
        }

        protected virtual object GetDataToSave()
        {
            return this;
        }

        protected abstract void LoadData(string fileName);
    }
}