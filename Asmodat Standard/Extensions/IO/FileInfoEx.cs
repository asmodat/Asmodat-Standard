using System;
using System.IO;
using System.Text;

namespace AsmodatStandard.Extensions.IO
{
    public static class FileInfoEx
    {
        public static FileInfo ToFileInfo(this string file) => file == null ? null : new FileInfo(file);

        public static void SortByName(this FileInfo[] infos, bool fullName = false)
        {
            if (fullName)
                infos.SortByFullName();
            else
                Array.Sort(infos, (f1, f2) => f1.Name.CompareTo(f2.Name));
        }

        public static void SortByFullName(this FileInfo[] infos)
            => Array.Sort(infos, (f1, f2) => f1.FullName.CompareTo(f2.FullName));

        public static void UnZip(this FileInfo info, string destination, Encoding encoding = null)
            => UnZip(info, new DirectoryInfo(destination), encoding);

        public static void UnZip(this FileInfo info, DirectoryInfo destination, Encoding encoding = null)
            => System.IO.Compression.ZipFile.ExtractToDirectory(info.FullName, destination.FullName, (encoding ?? Encoding.UTF8));
    }
}
