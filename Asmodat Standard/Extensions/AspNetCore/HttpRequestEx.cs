using AsmodatStandard.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Linq;
using AsmodatStandard.Extensions;

namespace AsmodatStandard.Extensions.AspNetCore
{
    public static class HttpRequestEx
    {
        public static string GetAuthorizationHeader(this HttpRequest request)
        {
            if (request == null || request.Headers.IsNullOrEmpty())
                throw new ArgumentException("HttpRequest was not detined or no HttpRequest.Headers were found.");

            if (!request.Headers.TryGetValue("Authorization", out var a1) &&
                !request.Headers.TryGetValue("authorization", out var a2))
                throw new Exception("GetAuthorizationHeader failure 'Authorization' nor 'authorization' headers were fond in HttpRequest.Headers.");


            return a1.FirstOrDefault() ?? a2.FirstOrDefault();
        }

        public static string GetBasicAuthSecret(this HttpRequest request) 
            => request.GetAuthorizationHeader()?.TrimStartMany("Bearer ", "bearer ").Replace(" ", "");

        public static (string login, string password) GetBasicAuthCredentials(this HttpRequest request)
        {
            var secret = request.GetBasicAuthSecret();

            if (!secret.IsBase64())
                throw new Exception("Basic Auth Secret must have Base64 format.");

            var decodedCredentials = secret.Base64Decode().Split(':');

            if (decodedCredentials.Length != 2)
                throw new Exception($"Decoded Basic Auth Secret must contain exactly one ':' separator, but had {decodedCredentials.Length}.");

            return (decodedCredentials[0], decodedCredentials[1]);
        }

        public static bool TryGetBasicAuthCredentials(this HttpRequest request, out string login, out string password, out Exception error)
        {
            try
            {
                var credentials = request.GetBasicAuthCredentials();
                login = credentials.login;
                password = credentials.password;
                error = null;
                return true;
            }
            catch(Exception ex)
            {
                login = null;
                password = null;
                error = ex;
                return false;
            }
        }
    }
}
