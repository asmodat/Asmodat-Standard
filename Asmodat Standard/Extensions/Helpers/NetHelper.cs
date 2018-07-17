using AsmodatStandard.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;

namespace AsmodatStandard.Extensions
{
    public static class NetHelper
    {
        public static bool IsPortFreeTCP(int port)
        {
            bool isAvailable = true;
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (var tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        public static int[] ListUsedPortsTCP()
        {
            var list = new List<int>();
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            return tcpConnInfoArray.Select(x => x.LocalEndPoint.Port).Distinct().ToArray();
        }
    }
}
