using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AsmodatStandard.Extensions;
using AsmodatStandard.Cryptography;

namespace AsmodatStandard.Types
{
    public static class SilyFileInfoEx
    {
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
    }

    public class SilyFileInfo
    {
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
        public string Name { get; set; }

        public string DirectoryFullName { get; set; }
        public string DirectoryName { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }

        public bool Exists { get; set; }
        public bool IsReadOnly { get; set; }

        public long Length { get; set; }
        
        public string MD5 { get; set; }
    }
}
