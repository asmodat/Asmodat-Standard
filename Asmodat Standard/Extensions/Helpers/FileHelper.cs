﻿using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions
{
    public static class FileHelper
    {
        public static void FileNamesReplace(string path, string to_replace, string to_replace_with, string seachPattern = "*", SearchOption options = SearchOption.AllDirectories)
        {
            var files = (new DirectoryInfo(path)).GetFiles(seachPattern, options).Where(file => file.Name.Contains(to_replace)).ToArray();

            Parallel.ForEach(files, file => {
                var new_name = file.Name.Replace(to_replace, to_replace_with);
                var new_path = Path.Combine(file.DirectoryName, new_name);
                File.Move(file.FullName, new_path);
            });
        }

        /// <summary>
        /// Deserializes Json Text File into .net type
        /// </summary>
        public static T DeserialiseJson<T>(string fileName, bool ungzip = false)
        {
           return JsonConvert.DeserializeObject<T>(
               ungzip ? 
               File.ReadAllText(fileName).UnGZip(Encoding.UTF8) : 
               File.ReadAllText(fileName));
        }

        /// <summary>
        /// Deserializes large json file.
        /// </summary>
        public static T DeserialiseJsonLarge<T>(string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, false))
            using (var jsonReader = new JsonTextReader(reader))
                return serializer.Deserialize<T>(jsonReader);
        }

        public static T DeserialiseJson<T>(FileInfo fi) => DeserialiseJson<T>(fi.FullName);

        public static bool IsEmptyOrWhiteSpace(string path) => new FileInfo(path).IsEmptyOrWhiteSpace();
        public static bool IsEmpty(string path) => new FileInfo(path).IsEmpty();

        public static bool IsEmptyOrWhiteSpace(this FileInfo fi) => string.IsNullOrWhiteSpace(File.ReadAllText(fi.FullName));
        public static bool IsEmpty(this FileInfo fi) => string.IsNullOrEmpty(File.ReadAllText(fi.FullName));

        public static string NameWithoutExtension(this FileInfo fi) => Path.GetFileNameWithoutExtension(fi.Name);


        /// <summary>
        /// overrides existing file or creates it and replaces the content with text using UTF8 encoding
        /// </summary>
        public static void WriteAllText(string fileName, string text, CompressionLevel compress = CompressionLevel.NoCompression)
        {
            using (var fw = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                if (!string.IsNullOrEmpty(text))
                {
                    var data = Encoding.UTF8.GetBytes(
                        compress == CompressionLevel.NoCompression ? 
                        text : text.GZip(Encoding.UTF8, compress));

                    fw.Write(data, 0, data.Length);
                }
        }
        
        public static void SerialiseJson(string fileName, object obj, Formatting formatting = Formatting.None, CompressionLevel compress = CompressionLevel.NoCompression)
            => WriteAllText(fileName, JsonConvert.SerializeObject(obj, formatting), compress);

        public static void SerialiseJsons<T>(string dir, IEnumerable<T> items, Func<T, string> nameSelector, Formatting formatting = Formatting.None, int maxDegreeOfParallelism = 10)
            => Parallel.ForEach(items, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, item => SerialiseJson(Path.Combine(dir, nameSelector(item)), item, formatting));

        public static IEnumerable<T> DeserializeJsons<T, K>(string dir, IEnumerable<K> items, Func<K, string> nameSelector, int maxDegreeOfParallelism = 100)
        {
            var results = new List<T>();
            Parallel.ForEach(items, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism } , item => {
                var path = Path.Combine(dir, $"{item}.json");
                results.Add(DeserialiseJson<T>(path));
            });
            return results.ToArray();
        }

        public static Dictionary<FileInfo,T> DeserializeJsons<T>(string path, string searchPattern = "*.json", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var di = new DirectoryInfo(path);
            var files = di.GetFiles(searchPattern, searchOption);
            var result = new Dictionary<FileInfo, T>();
            files?.ForEach(file => result.Add(file, DeserialiseJson<T>(file)));
            return result;
        }
    }
}
