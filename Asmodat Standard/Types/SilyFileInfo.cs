using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.IO;
using Newtonsoft.Json;

namespace AsmodatStandard.Types
{
    public static class SilyFileInfoEx
    {
        public static readonly object _locker = new object();

        public static SilyFileInfo ToSilyFileInfo(this FileInfo fi)
        {
            fi.Refresh();
            string md5 = null;
            if (fi.Exists)
                md5 = fi.MD5().ToHexString();

            return fi.ToSilyFileInfo(md5: md5);
        }

        public static SilyFileInfo ToSilyFileInfo(this FileInfo fi, string md5)
        {
            var sfi = new SilyFileInfo();
            sfi.Attributes = fi.Attributes;
            sfi.CreationTime = fi.CreationTimeUtc.ToUnixTimestamp();
            sfi.LastAccessTime = fi.LastAccessTimeUtc.ToUnixTimestamp();
            sfi.LastWriteTime = fi.LastWriteTimeUtc.ToUnixTimestamp();

            sfi.DirectoryFullName = fi.Directory.FullName;
            sfi.DirectoryName = fi.DirectoryName;
            sfi.Exists = fi.Exists;
            sfi.Extension = fi.Extension;
            sfi.FullName = fi.FullName;
            sfi.IsReadOnly = fi.IsReadOnly;
  
            sfi.Length = fi.Length;
            sfi.Name = fi.Name;
            sfi.MD5 = md5;

            return sfi;
        }

        public static bool FullNameEqual(string fn1, string fn2)
        {
            if (fn1 == null || fn2 == null)
                return false;

            var p1 = PathEx.ToLinuxPath(fn1.Trim().ToLower());
            var p2 = PathEx.ToLinuxPath(fn2.Trim().ToLower());
            return p1 == p2;
        }

        public static bool FullNameEqual(this SilyFileInfo si1, SilyFileInfo si2)
            => SilyFileInfoEx.FullNameEqual(si1?.FullName, si2?.FullName);

        public static bool FullNameEqual(this SilyFileInfo si1, FileInfo fi2)
            => SilyFileInfoEx.FullNameEqual(si1?.FullName, fi2?.FullName);

        public static bool FullNameEqual(this SilyFileInfo si1, string str)
            => SilyFileInfoEx.FullNameEqual(si1?.FullName, str);

        public static string TryGetProperty(this SilyFileInfo si, string key)
            => si?.Properties?.GetValueOrDefault(key, @default: null);

        /// <summary>
        /// Threadsafe set property, creates new dictionary if it does not already exists
        /// </summary>
        /// <returns>false if key was not added, true if it was added or replaced</returns>
        public static bool TrySetProperty(this SilyFileInfo si, string key, string value)
        {
            if (si == null || key.IsNullOrEmpty())
                return false;

            lock (_locker)
            {
                if (si.Properties.IsNullOrEmpty())
                    si.Properties = new Dictionary<string, string>();

                si.Properties[key] = value;
            }

            return true;
        }
    }

    public class SilyFileInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// unix timestamp
        /// </summary>
        public long CreationTime { get; set; }
        /// <summary>
        /// unix timestamp
        /// </summary>
        public long LastAccessTime { get; set; }
        /// <summary>
        /// unix timestamp
        /// </summary>
        public long LastWriteTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DirectoryFullName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DirectoryName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Extension { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        public bool Exists { get; set; }
        public bool IsReadOnly { get; set; }

        public long Length { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MD5 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Properties { get; set; }
    }
}
