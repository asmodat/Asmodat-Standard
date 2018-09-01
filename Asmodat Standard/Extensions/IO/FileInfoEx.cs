using System;
using System.IO;
using System.Text;

namespace AsmodatStandard.Extensions.IO
{
    public static class FileInfoEx
    {
        public static string ReadAllText(this FileInfo source, Encoding encoding = null)
            => encoding == null ? 
            File.ReadAllText(source.FullName) : 
            File.ReadAllText(source.FullName, encoding);

        public static void WriteAllText(this FileInfo source, string contents, Encoding encoding = null)
        {
            if (encoding == null)
                File.WriteAllText(source.FullName, contents);
            else
                File.WriteAllText(source.FullName, contents, encoding);
        }

        public static void Copy(this FileInfo source, FileInfo destination, bool @override = false)
            => File.Copy(source.FullName, destination.FullName, @override);

        public static bool TryDelete(this FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException($"{nameof(TryDelete)} failed, {nameof(fileInfo)} was null.");

            try
            {
                fileInfo.Delete();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static string ToLinuxPath(this string path)
        {
            if (path.IsNullOrEmpty())
                throw new ArgumentNullException($"{nameof(path)}");

            if (path.Length >= 2 && path[1] == ':')
                return "/" + path.ReplaceMany("/", "\\", ":");
            else
                return path.Replace("\\","/");
        }

        public static FileInfo ToFileInfo(this string file) => file == null ? null : new FileInfo(file);

        public static void SortByName(this FileInfo[] infos, bool fullName = false)
        {
            if (fullName)
                infos.SortByFullName();
            else
                Array.Sort(infos, (f1, f2) => f1.Name.ToLower().Trim(' ', '\\', '/').CompareTo(f2.Name.ToLower().Trim(' ', '\\', '/')));
        }

        public static void SortByFullName(this FileInfo[] infos)
            => Array.Sort(infos, (f1, f2) => f1.FullName.ToLower().Trim(' ', '\\', '/').CompareTo(f2.FullName.ToLower().Trim(' ', '\\', '/')));

        public static void UnZip(this FileInfo info, string destination, Encoding encoding = null)
            => UnZip(info, new DirectoryInfo(destination), encoding);

        public static void UnZip(this FileInfo info, DirectoryInfo destination, Encoding encoding = null)
            => System.IO.Compression.ZipFile.ExtractToDirectory(info.FullName, destination.FullName, (encoding ?? Encoding.UTF8));
    }
}
