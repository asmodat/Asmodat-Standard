using AsmodatStandard.Networking.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsmodatStandard.Networking.GitHub
{
    public partial class GitHubHelper
    {
        private GitHubRepoConfig _config { get; set; }

        public GitHubHelper(GitHubRepoConfig config)
        {
            _config = config;
        }

        public GitHubHelper(
            string user,
            string branch,
            string repository,
            string accessToken, 
            int maxRatePerHour = 1000,
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36")
        {
            _config = new GitHubRepoConfig(accessToken: accessToken, maxRatePerHour: maxRatePerHour)
            {
                user = user,
                branch = branch,
                repository = repository,
                userAgent = userAgent,
            };
        }
    }
}
