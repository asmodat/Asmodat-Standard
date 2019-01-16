using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AsmodatStandard.Extensions.IO
{
    public static class PathEx
    {
        public static string ToLinuxPath(this string path)
        {
            if (path.IsNullOrEmpty())
                throw new ArgumentNullException($"{nameof(path)}");

            path = path.TrimOrDefault(' ');

            if (path.IsNullOrWhitespace())
                return null;

            if (path.Length > 1 && path[1] == ':')
                path = "/" + path.SetChar(1, '/');

            path = path.Replace("\\", "/"); //

            while (path.Contains("//")) //trim multi slashes
                path = path.Replace("//", "/");

            while (path.Contains(" /")) //trim space slash'es
                path = path.Replace(" /", "/");

            while (path.Contains("/ ")) //trim slash spaces
                path = path.Replace("/ ", "/");

            return path.TrimOrDefault(' ');
        }

        public static string ToWindowsPath(this string path)
        {
            if (path.IsNullOrEmpty())
                throw new ArgumentNullException($"{nameof(path)}");

            path = path.TrimOrDefault(' ');

            if (path.IsNullOrWhitespace())
                return null;

            if (path.StartsWith("./"))
                path = path.Replace("./", "/");

            path = path.Replace("/", "\\"); //

            while (path.Contains("\\\\")) //trim multi slashes
                path = path.Replace("\\\\", "\\");

            while (path.Contains(" \\")) //trim space slash'es
                path = path.Replace(" \\", "\\");

            while (path.Contains("\\ ")) //trim slash spaces
                path = path.Replace("\\ ", "\\");

            return path.TrimOrDefault(' ');
        }

        public static string Combinewindows(params string[] paths)
        {
            paths = paths?.Where(x => !x.IsNullOrWhitespace()).ToArray();

            if (paths.IsNullOrEmpty())
                return null;

            var result = "";

            for (int i = 0; i < paths.Length; i++)
            {
                var p = paths[i].ToWindowsPath();

                if (p.IsNullOrEmpty())
                    continue;

                if (result.Length == 0)
                {
                    result = p;
                    continue;
                }
                else if (p.Length <= 1 && p == "\\")
                    continue;

                result = result.TrimEnd("\\") + "\\" + p.TrimStart("\\");
            }

            return result;
        }

        public static string CombineLinux(params string[] paths)
        {
            paths = paths?.Where(x => !x.IsNullOrWhitespace()).ToArray();

            if (paths.IsNullOrEmpty())
                return null;

            var result = "";

            for (int i = 0; i < paths.Length; i++)
            {
                var p = PathEx.ToLinuxPath(paths[i]);

                if (p.IsNullOrEmpty())
                    continue;

                if (result.Length == 0)
                {
                    result = p;
                    continue;
                }
                else if (p.Length <= 1 && p == "/")
                    continue;

                result = result.TrimEnd("/") + "/" + p.TrimStart("/");
            }

            return result;
        }


        public static string Combine(this DirectoryInfo info, params string[] paths)
            => PathEx.Combine(new string[] { info.FullName }.Merge(paths));

        public static string Combine(params string[] paths)
        {
            var list = new List<string>();

            if (paths != null)
                foreach (var p in paths)
                {
                    if (p.IsNullOrEmpty())
                        continue;

                    var path = p;

                    while (path.Contains("//")) //trim multi slashes
                        path = path.Replace("//", "/");

                    while (path.Contains(" /")) //trim space slash'es
                        path = path.Replace(" /", "/");

                    while (path.Contains("/ ")) //trim slash spaces
                        path = path.Replace("/ ", "/");

                    while (path.Contains("\\\\")) //trim multi slashes
                        path = path.Replace("\\\\", "\\");

                    while (path.Contains(" \\")) //trim space slash'es
                        path = path.Replace(" \\", "\\");

                    while (path.Contains("\\ ")) //trim slash spaces
                        path = path.Replace("\\ ", "\\");

                    path = path.TrimOrDefault(' ');

                    if (path.IsNullOrEmpty())
                        continue;

                    var s = path.ReplaceMany("", " ", "/", "\\");
                    if (s.IsNullOrEmpty() && list.Count > 0)
                        continue;

                    list.Add(path);
                }
            
            return Path.Combine(paths == null ? null : list.ToArray());
        }
    }
}
