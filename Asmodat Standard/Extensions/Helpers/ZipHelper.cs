using AsmodatStandard.Extensions.Collections;
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
    public static class ZipHelper
    {
        public static T DeserialiseJson<T>(string zipFile, string entryName)
            => JsonConvert.DeserializeObject<T>(ReadText(zipFile, entryName));

        public static IEnumerable<T> DeserialiseJsons<T, K>(string zipFile, IEnumerable<K> items, Func<K, string> entryNameSelector)
            => items.Select(item => DeserialiseJson<T>(zipFile, entryNameSelector(item))).ToArray();

        public static void SerialiseJson(string zipFile, string entryName, object obj, Formatting formatting = Formatting.None)
            => UpdateText(zipFile, entryName, JsonConvert.SerializeObject(obj, formatting));

        public static void SerialiseJsons<T>(string zipFile, IEnumerable<T> items, Func<T, string> entryNameSelector, Formatting formatting = Formatting.None)
            => items.ForEach(item => SerialiseJson(zipFile, entryNameSelector(item), item, formatting));


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
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, false))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                if (archive.Entries.Any(x => x.FullName == entryName))
                    archive.GetEntry(entryName).Delete();

                var entry = archive.CreateEntry(entryName);
                using (var writer = new StreamWriter(entry.Open()))
                    writer.Write(text);
            }
        }

        public static string ReadText(string path, string entryName)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = archive.GetEntry(entryName);
                using (var reader = new StreamReader(entry.Open()))
                    return reader.ReadToEnd();
            }
        }

        public static void Delete(string path, string entryName)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
                archive.GetEntry(entryName).Delete();
        }
    }
}
