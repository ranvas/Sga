using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace Serialization
{
    public static class Serializer
    {
        public static string ToJSON(object obj, JsonSerializerSettings settings)
        {
            var result = JsonConvert.SerializeObject(
                obj,
                Formatting.Indented,
                settings
            );
            return result;
        }

        public static string ToJSON(object? obj)
        {
            if (obj == null)
                return string.Empty;
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return ToJSON(obj, settings);
        }

        public static T DeserializeAnonymousType<T>(string json, T model)
        {
            return JsonConvert.DeserializeAnonymousType(json, model) ?? ThrowExceptionIfNull<T>(typeof(T));
        }

        public static T Deserialize<T>(string obj, string dateFormat = "")
        {
            if (string.IsNullOrEmpty(dateFormat))
            {
                return JsonConvert.DeserializeObject<T>(obj) ?? ThrowExceptionIfNull<T>(typeof(T));
            }

            return JsonConvert.DeserializeObject<T>(obj, new IsoDateTimeConverter { DateTimeFormat = dateFormat }) ?? ThrowExceptionIfNull<T>(typeof(T));
        }

        public static T ThrowExceptionIfNull<T>(Type type)
        {
            throw new Exception($"не получилось десериализовать {typeof(T).FullName}");
        }

        public static T DeserializeFromFile<T>(string filePath, string dateFormat = "")
        {
            return Deserialize<T>(File.ReadAllText(filePath), dateFormat);
        }

    }
}