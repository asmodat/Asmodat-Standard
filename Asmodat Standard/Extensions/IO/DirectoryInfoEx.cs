﻿using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.IO
{
    public static class DirectoryInfoEx
    {
        /// <summary>
        /// Tries to create directory
        /// This method throws is source is null
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true if directory exists or was created and false if not</returns>
        public static bool TryCreate(this DirectoryInfo source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.Exists)
                return true;

            try
            {
                if (!source.Exists)
                    source.Create();

                source.Refresh();
                Thread.Sleep(1);

                return source.Exists;
            }
            catch
            {
                return false;
            }
        }

        public static bool TrySetAttributes(this DirectoryInfo source, FileAttributes atributes, bool recursive, out Exception exception)
        {
            exception = null;
            if (source?.Exists != true)
                return false;

            var files = source.GetFiles(recursive: recursive);
            if (files.IsNullOrEmpty())
                return true;

            var success = true;
            foreach (var f in files)
            {
                try
                {
                    File.SetAttributes(f.FullName, atributes);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    success = false;
                }
            }

            return success;
        }

        public static bool TryDelete(this DirectoryInfo source, bool recursive)
            => source.TryDelete(recursive: recursive, out var ex);

        public static bool TryDelete(this DirectoryInfo source, bool recursive, out Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            exception = null;
            if (!source.Exists)
                return true;

            bool delete()
            {
                source.Refresh();
                if (source.Exists)
                    source.Delete(recursive: recursive);

                source.Refresh();
                Thread.Sleep(1);

                return source.Exists;
            }

            try
            {
                return delete();
            }
            catch (Exception outer)
            {
                Exception attributeException = null;
                try
                {
                    // sometimes file attributes have to be set to normal in order to delete files
                    source.TrySetAttributes(atributes: FileAttributes.Normal, recursive: recursive, out attributeException);
                    delete();
                }
                catch (Exception inner)
                {
                    exception = new AggregateException(outer, inner, attributeException);
                    return false;
                }

                exception = outer;
                return false;
            }
        }

        public static bool HasSubDirectory(this DirectoryInfo source, DirectoryInfo subDir)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.FullName == subDir.FullName)
                return true;

            var rootSourceDir = source?.Root?.FullName;
            var rootSubSourceDir = subDir?.Root?.FullName;

            if (rootSourceDir != rootSubSourceDir || rootSubSourceDir.IsNullOrEmpty() || rootSubSourceDir.IsNullOrEmpty())
                return false;

            var subDirFullName = subDir.FullName.TrimEnd('/', '\\');
            var sourceDirectory = source.FullName.ToDirectoryInfo();

            while (sourceDirectory != null && sourceDirectory.FullName.TrimEnd('/', '\\').Length >= subDirFullName.Length)
            {
                if (sourceDirectory.FullName.TrimEnd('/', '\\') == subDirFullName)
                    return true;

                sourceDirectory = sourceDirectory.Parent;
            }

            return false;
        }

        public static FileInfo[] GetFiles(this DirectoryInfo info, string pattern, bool recursive)
            => recursive ? info.GetFilesRecursive(new string[] { pattern }) : info.GetFiles(pattern);

        public static FileInfo[] GetFiles(this DirectoryInfo info, string[] patterns, bool recursive)
            => recursive ? info.GetFilesRecursive(patterns) : info.GetFiles(patterns);

        public static FileInfo[] GetFiles(this DirectoryInfo info, string[] patterns)
        {
            if (patterns == null)
                throw new ArgumentNullException($"{nameof(patterns)}");

            var bag = new ConcurrentBag<FileInfo>();

            patterns.ParallelForEach(pattern =>
            {
                var rootResult = info.GetFiles(pattern, SearchOption.TopDirectoryOnly);

                if (!rootResult.IsNullOrEmpty())
                    bag.AddRange(rootResult);
            });

            return bag.DistinctBy(x => x.FullName).ToArray();
        }

        public static FileInfo[] GetFiles(this DirectoryInfo info, bool recursive)
            => recursive ? info.GetFilesRecursive() : info.GetFiles();

        public static FileInfo[] GetFilesRecursive(this DirectoryInfo info)
            => GetFilesRecursive(info, new string[] { "*" });

        public static FileInfo[] GetFilesRecursive(this DirectoryInfo info, string[] patterns)
        {
            if (patterns == null)
                throw new ArgumentNullException($"{nameof(patterns)}");

            var bag = new ConcurrentBag<FileInfo>();
            patterns.ParallelForEach(pattern =>
            {
                var rootResult = info.GetFiles(pattern, SearchOption.TopDirectoryOnly);

                if (!rootResult.IsNullOrEmpty())
                    bag.AddRange(rootResult);

                foreach (var dir in info.GetDirectories())
                {
                    var leafResults = GetFilesRecursive(dir, new string[] { pattern });

                    if (!leafResults.IsNullOrEmpty())
                        bag.AddRange(leafResults);
                }
            });

            return bag.DistinctBy(x => x.FullName).ToArray();
        }

        public static FileInfo[] GetFiles(this DirectoryInfo info,
            bool recursive,
            IEnumerable<string> inclusivePatterns,
            IEnumerable<string> exclusivePatterns)
        {
            if (inclusivePatterns == null)
                throw new ArgumentNullException($"{nameof(inclusivePatterns)}");

            if (exclusivePatterns == null)
                throw new ArgumentNullException($"{nameof(exclusivePatterns)}");

            var locker = new object();
            var include = GetFiles(info, patterns: inclusivePatterns.ToArray(), recursive: recursive);
            var exclude = GetFiles(info, patterns: exclusivePatterns.ToArray(), recursive: recursive);

            var result = include.Where(iFile => !exclude.Any(eFile => eFile.FullName == iFile.FullName));
            return result.DistinctBy(x => x.FullName).ToArray();
        }

        public static DirectoryInfo[] GetDirectories(this DirectoryInfo info, bool recursive)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.Refresh();

            return recursive ? info.GetDirectoriesRecursive() : info.GetDirectories();
        }

        public static DirectoryInfo[] GetDirectoriesRecursive(this DirectoryInfo info)
        {
            var bag = new ConcurrentBag<DirectoryInfo>();
            var directories = info.GetDirectories();

            if (!directories.IsNullOrEmpty())
                bag.AddRange(directories);

            directories.ParallelForEach(directory =>
            {
                var arr = GetDirectoriesRecursive(directory);

                if (!arr.IsNullOrEmpty())
                    bag.AddRange(arr);
            });

            return bag.DistinctBy(x => x.FullName).ToArray();
        }

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

        public static async Task CopyAsync(this DirectoryInfo dir, DirectoryInfo destDirName, bool recursive, bool overwrite, int maxDegreeOfParallelism = 2)
        {
            if (!dir.Exists)
                return;

            var dirs = dir.GetDirectories();
            if (!destDirName.Exists)
                destDirName.Create();

            var files = dir.GetFiles();
            await ParallelEx.ForEachAsync(files, async file =>
            {
                string temppath = PathEx.RuntimeCombine(destDirName.FullName, file.Name);
                file.CopyTo(temppath, overwrite: overwrite);
                await Task.Delay(1);
            }, maxDegreeOfParallelism: maxDegreeOfParallelism);

            if (recursive)
                await ParallelEx.ForEachAsync(dirs, async subdir =>
                {
                    string temppath = PathEx.RuntimeCombine(destDirName.FullName, subdir.Name);
                    await CopyAsync(subdir.FullName.ToDirectoryInfo(), temppath.ToDirectoryInfo(), recursive: recursive, overwrite: overwrite);
                }, maxDegreeOfParallelism: maxDegreeOfParallelism);
        }
    }
}
