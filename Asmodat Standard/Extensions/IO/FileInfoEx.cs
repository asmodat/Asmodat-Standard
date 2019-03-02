using System;
using System.IO;
using System.Text;
using System.Threading;

namespace AsmodatStandard.Extensions.IO
{
    public static class FileInfoEx
    {
        public static void TrimEnd(this FileInfo source, long bytes)
        {
            using (FileStream fs = source.Open(FileMode.Open))
            {
                fs.SetLength(Math.Max(0, source.Length - bytes));
                fs.Close();
            }
        }

        public static bool TryCreate(this FileInfo source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.Exists)
                return true;

            try
            {
                if (!source.Directory.Exists)
                    source.Directory.Create();

                source.Create().Close();
                source.Refresh();
                Thread.Sleep(1);

                return source.Exists;
            }
            catch
            {
                return false;
            }
        }

        public static void AppendAllText(this FileInfo source, string contents, bool tryCreateSubdirectories = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if(tryCreateSubdirectories)
                source.TryCreate();

            File.AppendAllText(source.FullName, contents);
        }

        public static bool HasSubDirectory(this FileInfo source, DirectoryInfo subDir)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var subDirFullName = subDir.FullName;
            var sourceDirectory = source.Directory;

            var rootSourceDir = sourceDirectory?.Root?.FullName;
            var rootSubSourceDir = subDir?.Root?.FullName;

            if (rootSourceDir != rootSubSourceDir || rootSubSourceDir.IsNullOrEmpty() || rootSubSourceDir.IsNullOrEmpty())
                return false;

            while(sourceDirectory != null && sourceDirectory.FullName.Length >= subDirFullName.Length)
            {
                if (sourceDirectory.FullName == subDirFullName)
                    return true;

                sourceDirectory = sourceDirectory.Parent;
            }

            return false;
        }

        public static byte[] ReadAllBytes(this FileInfo source, Encoding encoding = null)
            => File.ReadAllBytes(source?.FullName);

        public static string ReadAllText(this FileInfo source, Encoding encoding = null)
            => encoding == null ? 
            File.ReadAllText(source.FullName) : 
            File.ReadAllText(source.FullName, encoding);

        public static void WriteAllText(this FileInfo source, string contents)
            => File.WriteAllText(source.FullName, contents);
        
        public static void WriteAllText(this FileInfo source, string contents, Encoding encoding)
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

        public static FileInfo ToFileInfo(this string file, bool throwIfNotFound)
        {
            if (file.IsNullOrEmpty())
                throw new ArgumentException($"File NOT found, file name is null or empty.");

            var fi = file.ToFileInfo();
            if (fi == null || !fi.Exists)
                throw new Exception($"File NOT found: '{fi?.FullName ?? null}'.");

            return fi;
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
