using System.IO;

using Newtonsoft.Json;

namespace CKTranslater.Storages
{
    public abstract class BaseDB<TStore>
        where TStore : class, new()
    {
        protected TStore items { get; private set; }
        private string fileName;

        protected BaseDB()
        {
        }

        protected static TDB LoadFromFile<TDB>(string fileName)
           where TDB : BaseDB<TStore>, new()
        {
            TStore items = null;

            if (File.Exists(fileName))
            {
                using StreamReader file = File.OpenText(fileName);
                JsonSerializer serializer = new JsonSerializer();
                items = (TStore)serializer.Deserialize(file, typeof(TStore));
            }

            return new TDB
            {
                items = items ?? new TStore(),
                fileName = fileName
            };
        }

        public void Save(string fileName = null)
        {
            using StreamWriter file = File.CreateText(fileName ?? this.fileName);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, this.items);
        }
    }
}
