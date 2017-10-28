using System.Net.Http;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions
{
    public static class HttpHelper
    {
        public static async Task<string> GET(string requestUri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requestUri);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
