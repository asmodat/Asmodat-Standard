using AsmodatStandard.Extensions.Collections;
using System;
using System.IO;

namespace AsmodatStandard.Extensions.IO
{
    public static class DirectoryInfoEx
    {
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
