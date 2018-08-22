using System.Runtime.InteropServices;

namespace AsmodatStandard.Extensions
{
    public static class RuntimeEx
    {
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsOSX() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}
