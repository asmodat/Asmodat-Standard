using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AsmodatStandard.Extensions
{
    public static class FileHelper
    { 
        /// <summary>
        /// Deserializes Json Text File into .net type
        /// </summary>
        public static T DeserialiseJson<T>(string fileName) => JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));

        public static T DeserialiseJson<T>(FileInfo fi) => DeserialiseJson<T>(fi.FullName);

        public static bool IsEmptyOrWhiteSpace(string path) => new FileInfo(path).IsEmptyOrWhiteSpace();
        public static bool IsEmpty(string path) => new FileInfo(path).IsEmpty();

        public static bool IsEmptyOrWhiteSpace(this FileInfo fi) => string.IsNullOrWhiteSpace(File.ReadAllText(fi.FullName));
        public static bool IsEmpty(this FileInfo fi) => string.IsNullOrEmpty(File.ReadAllText(fi.FullName));


        public static string NameWithoutExtension(this FileInfo fi) => Path.GetFileNameWithoutExtension(fi.Name);


        /// <summary>
        /// overrides existing file or creates it and replaces the content with text using UTF8 encoding
        /// </summary>
        public static void WriteAllText(string fileName, string text)
        {
            using (var fw = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                if (!string.IsNullOrEmpty(text))
                {
                    var data = Encoding.UTF8.GetBytes(text);
                    fw.Write(data, 0, data.Length);
                }
        }

        public static void SerialiseJson(string fileName, object obj, Formatting formatting = Formatting.None)
            => FileHelper.WriteAllText(fileName, JsonConvert.SerializeObject(obj, formatting));
    }
}
