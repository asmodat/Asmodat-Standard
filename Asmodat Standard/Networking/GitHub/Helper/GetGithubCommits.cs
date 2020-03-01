using AsmodatStandard.Extensions;
using AsmodatStandard.Networking.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking.GitHub
{
    public partial class GitHubHelper
    {
        public async Task<GitHubRepoCommits> GetGitHubCommits()
        {
            var request = $"https://api.github.com/repos/{_config.user}/{_config.repository}/commits/{_config.branch}";
            return await HttpHelper.GET<GitHubRepoCommits>(
                                    requestUri: request,
                                    ensureStatusCode: System.Net.HttpStatusCode.OK,
                                    defaultHeaders: new (string, string)[] {
                                        ("User-Agent", _config.userAgent),
                                        ("Authorization", $"token {await _config.GetAccessToken()}")
                                    });
        }
    }
}
