using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Net
{
    public static class DnsEx
    {
        public static async Task<string[]> GetHostAddressesAsync(string host, int timeout)
        {
            Exception lastException = null;
            var sw = Stopwatch.StartNew();
            string[] hostAddressess = null;
            do
            {
                try
                {
                    hostAddressess = (await Dns.GetHostAddressesAsync(host))?
                        .Where(x => (x?.ToString()).IsNullOrEmpty() == false)
                        .Select(x => x.ToString()).ToArray();

                    if (!hostAddressess.IsNullOrEmpty())
                        return hostAddressess;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }

                await Task.Delay(1000);

            } while (sw.ElapsedMilliseconds < timeout);

            throw new Exception($"Timeout, Could not find Addressess for DNS Host Name: '{host}', Elapsed: {sw.ElapsedMilliseconds}/{timeout} [ms].", lastException);
        }

        public static string GetHostName(string uri, int timeout)
        {
            Exception lastException = null;
            var sw = Stopwatch.StartNew();
            string hostName = null;
            do
            {
                try
                {
                    hostName = Dns.GetHostEntry(uri).HostName;

                    if (!hostName.IsNullOrWhitespace())
                        return hostName;
                }
                catch(Exception ex)
                {
                    lastException = ex;
                }

                Thread.Sleep(1000);

            } while (sw.ElapsedMilliseconds < timeout);

            throw new Exception($"Timeout, Could not find DNS Host Name for address: '{uri}', Elapsed: {sw.ElapsedMilliseconds}/{timeout} [ms].", lastException);
        }

        public static string GetHostNameOrDefault(string uri, string @default)
        {
            try
            {
                return Dns.GetHostEntry(uri).HostName;
            }
            catch
            {
                return @default;
            }
        }
    }
}
