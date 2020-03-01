using AsmodatStandard.Extensions;
using AsmodatStandard.Networking.GitHub.Models;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking.GitHub
{
    public partial class GitHubHelper
    {
        public async Task<GitHubBlob> GetGitHubBlob(GitHubObject obj)
        {
            return await HttpHelper.GET<GitHubBlob>(
                                    requestUri: obj.url,
                                    ensureStatusCode: System.Net.HttpStatusCode.OK,
                                    defaultHeaders: new (string, string)[] {
                                        ("User-Agent", _config.userAgent),
                                        ("Authorization", $"token {await _config.GetAccessToken()}")
                                    });
        }
    }
}
