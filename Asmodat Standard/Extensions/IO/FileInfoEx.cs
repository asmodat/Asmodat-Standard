using AsmodatStandard.Cryptography;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace AsmodatStandard.Extensions.IO
{
    public static class FileInfoEx
    {
        public static FileInfo Zip(this FileInfo source, string destination = null)
        {
            if (source?.Exists != true)
                throw new Exception("Can't Zip 'source' was not defined.");

            if (destination.IsNullOrEmpty())
                destination = source.FullName + ".zip";

            var zip = destination.ToFileInfo();
            if (!zip.TryDelete() && !zip.Directory.TryCreate())
                throw new Exception($"Zipping source file '{source?.FullName ?? "undefined"}' failed, could not remove '{zip?.FullName}' or create '{zip?.Directory}'.");

            return source.Zip(destination);
        }

        public static FileInfo Zip(this FileInfo source, FileInfo destination)
        {
            using (FileStream fs = new FileStream(destination.FullName, FileMode.Create))
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                arch.CreateEntryFromFile(source.FullName, source.Name, CompressionLevel.Optimal);

            return destination;
        }

        public static FileStream ZipStream(this FileInfo source, FileShare share)
        {
            var fs = File.Open(source.FullName,
                            FileMode.Open,
                            FileAccess.Read,
                            share);
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: true))
                arch.CreateEntryFromFile(source.FullName, source.Name, CompressionLevel.Optimal);

            return fs;
        }

        public static void UnZipStream(this FileInfo destination, Stream s)
        {
            if (!destination.Directory.TryCreate())
                throw new Exception($"Failed UnZipStream, directory '{destination?.Directory?.FullName ?? "undefined"}' does not exist or coudn't be created.");

            using (ZipArchive arch = new ZipArchive(s, ZipArchiveMode.Read))
            {
                if (arch.Entries.Count != 1)
                    throw new Exception($"UnZipStream to file only accepts single entry, but was '{arch.Entries.Count}'.");

                var en = arch.Entries[0];
                en.ExtractToFile(destination.FullName);
            }
        }

        public static void UnZip(this FileInfo source, string destination = null)
        {
            DirectoryInfo di = null;
            if (destination == null)
                di = source?.Directory;
            else
                di = destination?.ToDirectoryInfo();

            UnZip(source, di);
        }

        public static void UnZip(this FileInfo source, DirectoryInfo destination)
        {
            if (!source.Exists)
                throw new Exception($"Failed UnZip, source '{source.FullName ?? "undefined"}' was not found");

            if (!destination.TryCreate())
                throw new Exception($"Failed to unzip '{source.FullName ?? "undefined"}', couldn't create '{destination.FullName??"undefined"}'");

            using (var fs = source.OpenRead())
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Read))
                arch.ExtractToDirectory(destination.FullName);
        }

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

        public static bool TryClear(this FileInfo fileInfo)
            => fileInfo.TryDelete() && fileInfo.TryCreate();

        public static bool TryDelete(this FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException($"{nameof(TryDelete)} failed, {nameof(fileInfo)} was null.");

            fileInfo.Refresh();
            if (!fileInfo.Exists)
                return true;

            try
            {
                fileInfo.Delete();
                fileInfo.Refresh();
                return fileInfo.Exists == false;
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

        public static void AppendAllText(this FileInfo source, string contents)
            => File.AppendAllText(source.FullName, contents);
    }
}
