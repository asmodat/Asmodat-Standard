using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AsmodatStandard.Extensions
{
    public static class ProcessEx
    {
        /// <summary>
        /// https://github.com/dotnet/corefx/issues/10361
        /// </summary>
        /// <param name="url"></param>
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeEx.IsWindows())
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeEx.IsLinux())
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeEx.IsOSX())
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
