using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AsmodatStandard.Extensions;

namespace AsmodatStandard.Types
{
    public static class DirectoryFileInfoEx
    {
        public static SilyDirectoryInfo ToSilyDirectoryInfo(this DirectoryInfo di)
        {
            if (di == null)
                return null;

            var sdi = new SilyDirectoryInfo();
            sdi.Attributes = di.Attributes;
            sdi.CreationTime = di.CreationTimeUtc.ToUnixTimestamp();
            sdi.LastAccessTime = di.LastAccessTimeUtc.ToUnixTimestamp();
            sdi.LastWriteTime = di.LastWriteTimeUtc.ToUnixTimestamp();
            sdi.Exists = di.Exists;
            sdi.Extension = di.Extension;
            sdi.FullName = di.FullName;
            sdi.Name = di.Name;

            return sdi;
        }
    }

    public class SilyDirectoryInfo
    {
        public FileAttributes Attributes { get; set; }

        public long CreationTime { get; set; }
        public long LastAccessTime { get; set; }
        public long LastWriteTime { get; set; }
        public string Name { get; set; }

        public string Extension { get; set; }
        public string FullName { get; set; }

        public bool Exists { get; set; }
        
    }
}
