using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AsmodatStandard.Extensions
{
    public static class EnvironmentEx
    {
        public static DirectoryInfo HomePath()
        {
            var path = (RuntimeEx.IsWindows()) ?
                Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") :
                Environment.GetEnvironmentVariable("HOME");

            return new DirectoryInfo(path);
        }
    }
}
