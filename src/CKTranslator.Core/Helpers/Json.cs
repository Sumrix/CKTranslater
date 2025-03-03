﻿using Newtonsoft.Json;

using System.Threading.Tasks;

namespace CKTranslator.Core.Helpers
{
    public static class Json
    {
        public static string Stringify(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run<string>(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }

        public static T ToObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static async Task<T> ToObjectAsync<T>(string value)
        {
            return await Task.Run<T>(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }
    }
}