using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Networking.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking.GitHub
{
    public partial class GitHubHelper
    {
        public Task<GitHubTree> GetGitHubTrees(GitHubObject go) => GetGitHubTrees(go?.url);
        public async Task<GitHubTree> GetGitHubTrees(string request = null)
        {
            request = request.IsNullOrEmpty() ? $"https://api.github.com/repos/{_config.user}/{_config.repository}/git/trees/{_config.branch}" : request;

            var root = await HttpHelper.GET<GitHubTree>(
                                    requestUri: request,
                                    ensureStatusCode: System.Net.HttpStatusCode.OK,
                                    defaultHeaders: new (string, string)[] {
                                        ("User-Agent", _config.userAgent),
                                        ("Authorization", $"token {await _config.GetAccessToken()}")
                                    });

            if (!(root?.tree).IsNullOrEmpty())
                foreach(var o in root.tree)
                    if(o.IsTree())
                        o.tree = await this.GetGitHubTrees(o);

            return root;
        }

    }
}
