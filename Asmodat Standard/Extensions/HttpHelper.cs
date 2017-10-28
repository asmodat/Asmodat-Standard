using AsmodatStandard.Extensions.Threading;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AsmodatStandard.Extensions
{
    public static class HttpHelper
    {
        public static string GET(string requestUri)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(requestUri).Await();
                return response.Content.ReadAsStringAsync().Await();
            }
        }
    }
}
