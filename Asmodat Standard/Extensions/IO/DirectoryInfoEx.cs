using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace AsmodatStandard.Extensions.IO
{
    public static class DirectoryInfoEx
    {
        public static FileInfo[] GetFilesRecursive(this DirectoryInfo info)
        {
            var files = new List<FileInfo>();
            files.AddRange(info.GetFiles());

            foreach(var dir in info.GetDirectories())
                files.AddRange(GetFilesRecursive(dir));

            return files.ToArray();
        }

        public static DirectoryInfo[] GetDirectoriesRecursive(this DirectoryInfo info)
        {
            var result = new List<DirectoryInfo>();
            var directories = info.GetDirectories();
            result.AddRange(directories);

            foreach (var dir in directories)
                result.AddRange(GetDirectoriesRecursive(dir));

            return result.ToArray();
        }

        public static string Combine(this DirectoryInfo info, params string[] paths)
            => Path.Combine(new string[] { info.FullName }.Merge(paths));

        public static DirectoryInfo ToDirectoryInfo(this string dir) => dir == null ? null : new DirectoryInfo(dir);

        public static void SortByName(this DirectoryInfo[] infos, bool fullName = false)
        {
            if (fullName)
                infos.SortByFullName();
            else
                Array.Sort(infos, (f1, f2) => f1.Name.ToLower().Trim(' ', '\\', '/').CompareTo(f2.Name.ToLower().Trim(' ', '\\', '/')));
        }

        public static void SortByFullName(this DirectoryInfo[] infos)
            => Array.Sort(infos, (f1, f2) => f1.FullName.ToLower().Trim(' ', '\\', '/').CompareTo(f2.FullName.ToLower().Trim(' ', '\\', '/')));
    }
}
