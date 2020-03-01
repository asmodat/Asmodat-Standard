using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AsmodatStandard.Extensions
{
    public static class RuntimeEx
    {
        public static DirectoryInfo HomePath()
        {
            var path = (IsWindows()) ?
                Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") :
                Environment.GetEnvironmentVariable("HOME");

            return new DirectoryInfo(path);
        }
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsOSX() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}
