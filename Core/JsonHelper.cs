using System;
using System.IO;
using Newtonsoft.Json;

namespace Core
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return Activator.CreateInstance<T>();
            }

            using StreamReader file = File.OpenText(fileName);
            JsonSerializer serializer = new JsonSerializer();
            return (T) serializer.Deserialize(file, typeof(T));
        }

        public static string JsonPrettify(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using StringWriter stringWriter = new StringWriter();
            JsonTextReader jsonReader = new JsonTextReader(stringReader);
            JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }

        public static void Populate(string fileName, object obj)
        {
            using StreamReader file = File.OpenText(fileName);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(file, obj);
        }

        public static void Serialize(object obj, string fileName)
        {
            using StreamWriter file = File.CreateText(fileName);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            JsonSerializer serializer = JsonSerializer.Create(settings);
            serializer.Serialize(file, obj);
        }
    }
}