using AsmodatStandard.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions
{
    public static class HttpHelper
    {
        public static async Task<T> GET<T>(string requestUri, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => await SEND<T>(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<T> POST<T>(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => await SEND<T>(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<T> PUT<T>(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => await SEND<T>(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<string> GET(string requestUri, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null) 
            => await SEND(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> POST(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null) 
            => await SEND(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> PUT(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null) 
            => await SEND(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> SEND(HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => await SEND<string>(method, requestUri, content, ensureStatusCode, defaultHeaders);

        public static async Task<T> SEND<T>(HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                if (content != null)
                    request.Content = content; 

                using (var client = new HttpClient())
                {
                    defaultHeaders?.ForEach(header => { client.DefaultRequestHeaders.Add(header.key, header.value); });

                    var response = await client.SendAsync(request);
                    var responseContentJson = await response.Content.ReadAsStringAsync();

                    if (ensureStatusCode != null && response.StatusCode != ensureStatusCode.Value)
                        throw new Exception($"Request failed: '{requestUri}', expected status code to be: '{ensureStatusCode.Value}', but was: '{response.StatusCode}'. Content Body: '{responseContentJson}'");

                    return typeof(T) == typeof(string) ? (T)(object)responseContentJson : JsonConvert.DeserializeObject<T>(responseContentJson);
                }
            }
        }
    }
}
