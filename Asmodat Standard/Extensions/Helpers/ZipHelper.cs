using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AsmodatStandard.Extensions
{
    public static class ZipHelper
    {
        public static T DeserialiseJson<T>(string zipFile, string entryName)
            => JsonConvert.DeserializeObject<T>(ReadText(zipFile, entryName));

        public static IEnumerable<T> DeserializeJsons<T, K>(string zipFile, IEnumerable<K> items, Func<K, string> entryNameSelector)
            => Read<T>(zipFile, items.Select(item => entryNameSelector(item)));

        public static void SerialiseJson(string zipFile, string entryName, object obj, Formatting formatting = Formatting.None)
            => UpdateText(zipFile, entryName, JsonConvert.SerializeObject(obj, formatting));

        public static void SerialiseJsons<T>(string zipFile, IEnumerable<T> items, Func<T, string> entryNameSelector, Formatting formatting = Formatting.None)
            => UpdateText(zipFile, items.Select(item => (entryNameSelector(item), JsonConvert.SerializeObject(item, formatting))).ToArray());

        /// <summary>
        /// If 'path' doesn't exist creates empty zip file
        /// </summary>
        /// <param name="path"></param>
        public static void CreateEmpty(string path)
        {
            if (File.Exists(path))
                return;

            var data = new byte[]{80,75,5,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

            using (var fs = File.Create(path))
            {
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
            }
        }

        public static string[] GetEntriesFullNames(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, false))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                return archive.Entries?.Select(x => x.FullName)?.ToArray() ?? new string[0];
        }

        public static void UpdateText(string path, string entryName, string text)
            => UpdateText(path, (entryName, text));

        public static void UpdateText(string path, params (string name, string text)[] entries)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, false))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update, false, Encoding.UTF8))
                foreach (var txtEntry in entries)
                {
                    archive.GetEntry(txtEntry.name)?.Delete(); //remove if exists already
                    var entry = archive.CreateEntry(txtEntry.name, CompressionLevel.NoCompression);
                    using (var writer = new StreamWriter(entry.Open()))
                        writer.Write(txtEntry.text);
                }
        }

        public static string ReadText(string path, string entryName)
            => ReadText(path, new string[] { entryName }).First();

        public static IEnumerable<string> ReadText(string path, params string[] entryNames)
            => Read<string>(path, entryNames);

        public static IEnumerable<T> Read<T>(string path, IEnumerable<string> entryNames, int procesSplit = 5)
        {
            var isStringType = typeof(T) == typeof(string);
            var entryNamesSplits = entryNames.Split(procesSplit);
            var bag = new ConcurrentBag<List<T>>();
            var serializer = new JsonSerializer();
            var encoding = Encoding.UTF8;

            Parallel.ForEach(entryNamesSplits, new ParallelOptions { MaxDegreeOfParallelism = procesSplit }, entryNamesSplit =>
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    var results = new List<T>();
                    entryNamesSplit.Select(x => archive.GetEntry(x)).ForEach(entry =>
                    {
                        if (entry != null)
                            using (var reader = new StreamReader(entry.Open(), encoding, false, 4096, false))
                            {
                                if (isStringType)
                                    results.Add((T)(object)reader.ReadToEnd());
                                else
                                    using (var jsonReader = new JsonTextReader(reader))
                                        results.Add(serializer.Deserialize<T>(jsonReader));
                            }
                    });
                    bag.Add(results);
                }
            });

            return bag.SelectMany(x => x);
        }

        public static void Delete(string path, string entryName)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
                archive.GetEntry(entryName).Delete();
        }
    }
}

/*
 public static IEnumerable<T> Read<T>(string path, IEnumerable<string> entryNames)
        {
            var isStringType = typeof(T) == typeof(string);
            var results = new List<T>();

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096 * 32, true))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var serializer = new JsonSerializer();
                var encoding = Encoding.UTF8;
                entryNames.Select(x => archive.GetEntry(x)).ForEach(entry =>
                {
                    if (entry != null)
                        using (var reader = new StreamReader(entry.Open(), encoding, false, 4096, false))
                        {
                            if (isStringType)
                                results.Add((T)(object)reader.ReadToEnd());
                            else
                                using (var jsonReader = new JsonTextReader(reader))
                                    results.Add(serializer.Deserialize<T>(jsonReader));
                        }
                });
            }

            return results;
        }
     
     */
