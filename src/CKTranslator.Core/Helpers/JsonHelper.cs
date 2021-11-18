using Newtonsoft.Json;

using System;
using System.IO;

namespace CKTranslator.Core
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
            JsonSerializer serializer = new();
            object? obj = serializer.Deserialize(file, typeof(T));

            return obj is T tObj
                ? tObj
                : Activator.CreateInstance<T>();
        }

        public static string JsonPrettify(string json)
        {
            using StringReader stringReader = new(json);
            using StringWriter stringWriter = new();
            JsonTextReader jsonReader = new(stringReader);
            JsonTextWriter jsonWriter = new(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }

        public static void Populate(string fileName, object obj)
        {
            using StreamReader file = File.OpenText(fileName);
            JsonSerializer serializer = new();
            serializer.Populate(file, obj);
        }

        public static void Serialize(object obj, string fileName)
        {
            using StreamWriter file = File.CreateText(fileName);
            JsonSerializerSettings settings = new()
            {
                Formatting = Formatting.Indented
            };
            JsonSerializer serializer = JsonSerializer.Create(settings);
            serializer.Serialize(file, obj);
        }
    }
}