using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class JsonlExtensions
    {
        public static readonly JsonSerializerSettings Jss = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter( ) }
        };

        public static string MyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string BaseFolder = @$"{MyDocumentsFolder}\LINQPad Data\scd";

        public static IEnumerable<T> LoadFromJsonLines<T>(string fileName)
        {
            var filePath = Path.Combine(BaseFolder, fileName);
            var jsonl = File.ReadAllLines(filePath);
            var result = jsonl.Select(line =>
                JsonConvert.DeserializeObject<T>(line, Jss) ?? throw new InvalidOperationException());
            return result;
        }

        public static void SaveToJsonLines<T>(this IEnumerable<T> source, string fileName)
        {
            var jsonLines = source.Select(e => JsonConvert.SerializeObject(e, Jss));
            var filePath = Path.Combine(BaseFolder, fileName);
            File.WriteAllLines(filePath, jsonLines);
        }

        public static T LoadFromJson<T>(string fileName)
        {
            var filePath = Path.Combine(BaseFolder, fileName);
            var json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<T>(json, Jss) ?? throw new InvalidOperationException();
            return result;
        }
    }
}