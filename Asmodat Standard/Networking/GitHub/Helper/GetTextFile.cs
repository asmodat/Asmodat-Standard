using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Networking.GitHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking.GitHub
{
    public partial class GitHubHelper
    {
        public async Task<T[]> GetObjects<T>(string path, string filename, bool throwIfNotFound = true)
        {
            var files = await GetGitHubTrees();
            var instanceObjects = files.GetObjectsByPath(path: path)?.Where(x => x.IsTree());

            if (instanceObjects.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw new System.Exception($"No files were found in the `{path ?? "/"}` directory");
                else
                    return null;
            }

            var results = new List<T>();
            foreach (var instanceObject in instanceObjects)
            {
                var configObject = instanceObject?.objects.FirstOrDefault(x => x.path.ToLower() == filename);

                if (configObject?.IsBlob() != true)
                    continue;

                var blob = await GetGitHubBlob(configObject);
                var obj = blob.JsonDeserialize<T>();
                results.Add(obj);
            }

            if (results.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw new System.Exception($"No files with the name `{filename}` were found in the `{path ?? "/"}` directory");
                else
                    return null;
            }

            return results.ToArray();
        }

        public async Task<T> GetObject<T>(string path, string filename, bool throwIfNotFound = true)
        {
            var results = await GetObjects<T>(path: path, filename: filename, throwIfNotFound: throwIfNotFound);

            if (results.Length != 1)
                throw new System.Exception($"Found {results.Length} files with the name `{filename}` in the `{path ?? "/"}` directory but expected 1");

            return results[0];
        }
    }
}
