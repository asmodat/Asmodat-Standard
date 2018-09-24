using AsmodatStandard.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AsmodatStandard.Extensions
{
    public static class HttpHelper
    {
        public class CurlResponse
        {
            public CurlResponse(HttpRequestMessage Request,
               HttpResponseMessage Response,
               HttpClient Client)
            {
                this.Request = Request;
                this.Response = Response;
                this.Client = Client;
            }

            public readonly HttpRequestMessage Request;
            public readonly HttpResponseMessage Response;
            public readonly HttpClient Client;
        }

        public static HttpClient NewNetworkClient(string user, string password) 
            => new HttpClient(new HttpClientHandler()
            {
                Credentials = new NetworkCredential(user, password)
            });

        public static string UriEncode(this string str) => HttpUtility.UrlEncode(str);
        public static string UriDecode(this string str) => HttpUtility.UrlDecode(str);

        public static string HtmlEncode(this string str) => HttpUtility.HtmlEncode(str);
        public static string HtmlDecode(this string str) => HttpUtility.HtmlDecode(str);

        public static async Task<T> GET<T>(Uri uri, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await GET<T>(uri.ToString(), ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<T> GET<T>(string requestUri, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => await SEND<T>(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<T> POST<T>(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await SEND<T>(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<T> POST<T>(string requestUri, string content, Encoding encoding, string mediaType, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await SEND<T>(HttpMethod.Post, requestUri, content: new StringContent(content, encoding, mediaType), ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<T> PUT<T>(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await SEND<T>(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<string> GET(string requestUri, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders) 
            => await SEND(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> POST(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders) 
            => await SEND(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<string> DELETE(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await SEND(HttpMethod.Delete, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);
        public static async Task<T> DELETE<T>(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await SEND<T>(HttpMethod.Delete, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<string> PUT(string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders) 
            => await SEND(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> SEND(HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await SEND<string>(method, requestUri, content, ensureStatusCode, addHeadersWithoutValidation, defaultHeaders);

        public static async Task<T> SEND<T>(HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
        {
            var result = await CURL(
                               method: method,
                               requestUri: requestUri,
                               content: content,
                               ensureStatusCode: ensureStatusCode,
                               addHeadersWithoutValidation: addHeadersWithoutValidation,
                               defaultHeaders: defaultHeaders);

            var responseContentJson = await result.Response.Content?.ReadAsStringAsync();

            if (ensureStatusCode != null && result.Response.StatusCode != ensureStatusCode.Value)
                throw new Exception($"Request failed: '{requestUri}', expected status code to be: '{ensureStatusCode.Value}', but was: '{result.Response.StatusCode}'. Content Body: '{responseContentJson}'");

            return typeof(T) == typeof(string) ? (T)(object)responseContentJson : JsonConvert.DeserializeObject<T>(responseContentJson);
        }

        public static Task<CurlResponse> CURL(HttpMethod method, Uri url, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => CURL(
               method: method,
               requestUri: url.ToString(),
               content: content,
               ensureStatusCode: ensureStatusCode,
               defaultHeaders: defaultHeaders);

        public static async Task<CurlResponse> CURL(HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, (string key, string value)[] defaultHeaders = null)
        {
            var request = new HttpRequestMessage(method, requestUri);

            if (content != null)
                request.Content = content;

            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(150) };

           
            defaultHeaders?.ForEach(header => {
                if(addHeadersWithoutValidation)
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.key, header.value);
                else
                    client.DefaultRequestHeaders.Add(header.key, header.value);
            });
            return new CurlResponse(request, await client.SendAsync(request), client);
        }

        public static async Task<T> GET<T>(this HttpClient client, Uri uri, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.GET<T>(uri.ToString(), ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation,defaultHeaders: defaultHeaders);

        public static async Task<T> GET<T>(this HttpClient client, string requestUri, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, (string key, string value)[] defaultHeaders = null)
            => await client.SEND<T>(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<T> POST<T>(this HttpClient client, string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params(string key, string value)[] defaultHeaders)
            => await client.SEND<T>(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);
        public static async Task<T> POST<T>(this HttpClient client, string requestUri, string content, Encoding encoding, string mediaType, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.SEND<T>(HttpMethod.Post, requestUri, content: new StringContent(content, encoding, mediaType), ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<T> PUT<T>(this HttpClient client, string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.SEND<T>(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<string> GET(this HttpClient client, string requestUri, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await client.SEND(HttpMethod.Get, requestUri, content: null, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);

        public static async Task<string> POST(this HttpClient client, string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.SEND(HttpMethod.Post, requestUri, content: content, ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);
        public static async Task<string> POST(this HttpClient client, string requestUri, string content, Encoding encoding, string mediaType,  HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.SEND(HttpMethod.Post, requestUri, content: new StringContent(content, encoding, mediaType), ensureStatusCode: ensureStatusCode, addHeadersWithoutValidation: addHeadersWithoutValidation, defaultHeaders: defaultHeaders);

        public static async Task<string> PUT(this HttpClient client, string requestUri, HttpContent content, HttpStatusCode? ensureStatusCode = null, params (string key, string value)[] defaultHeaders)
            => await client.SEND(HttpMethod.Put, requestUri, content: content, ensureStatusCode: ensureStatusCode, defaultHeaders: defaultHeaders);
        public static async Task<string> SEND(this HttpClient client, HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
            => await client.SEND<string>(method, requestUri, content, ensureStatusCode, addHeadersWithoutValidation, defaultHeaders);

        public static async Task<T> SEND<T>(this HttpClient client, HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, params (string key, string value)[] defaultHeaders)
        {
            var result = await client.CURL(
                               method: method,
                               requestUri: requestUri,
                               content: content,
                               ensureStatusCode: ensureStatusCode,
                               addHeadersWithoutValidation: addHeadersWithoutValidation,
                               defaultHeaders: defaultHeaders);

            var responseContentJson = await result.Response.Content?.ReadAsStringAsync();

            if (ensureStatusCode != null && result.Response.StatusCode != ensureStatusCode.Value)
                throw new Exception($"Request failed: '{requestUri}', expected status code to be: '{ensureStatusCode.Value}', but was: '{result.Response.StatusCode}'. Content Body: '{responseContentJson}'");

            return typeof(T) == typeof(string) ? (T)(object)responseContentJson : JsonConvert.DeserializeObject<T>(responseContentJson);
        }

        public static Task<CurlResponse> CURL(this HttpClient client, HttpMethod method, Uri url, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, (string key, string value)[] defaultHeaders = null)
            => client.CURL(
               method: method,
               requestUri: url.ToString(),
               content: content,
               ensureStatusCode: ensureStatusCode,
               defaultHeaders: defaultHeaders);

        public static async Task<CurlResponse> CURL(this HttpClient client, HttpMethod method, string requestUri, HttpContent content = null, HttpStatusCode? ensureStatusCode = null, bool addHeadersWithoutValidation = false, (string key, string value)[] defaultHeaders = null)
        {
            var request = new HttpRequestMessage(method, requestUri);

            if (content != null)
                request.Content = content;

            defaultHeaders?.ForEach(header => {
                if (addHeadersWithoutValidation)
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.key, header.value);
                else
                    client.DefaultRequestHeaders.Add(header.key, header.value);
            });

            return new CurlResponse(request, await client.SendAsync(request), client);
        }
    }
}
