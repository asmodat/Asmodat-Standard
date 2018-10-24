using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using AsmodatStandard.Threading;

namespace AsmodatStandard.Cryptography
{
    public static partial class HashHelper
    {
        public static bool IsValidSHA256Hex(this string digest)
            => !digest.IsNullOrEmpty() && Regex.Match(input: digest, pattern: "\\b[A-Fa-f0-9]{64}\\b").Success;

        public static byte[] SHA256(this string str, Encoding encoding = null)
            => str.ToByteArray(encoding ?? Encoding.UTF8).SHA256();

        public static byte[] SHA256(this byte[] ba)
        {
            using (var hm = new SHA256Managed())
                return hm.ComputeHash(ba);
        }

        /// <summary>
        /// Calculates SHA256 of the file content
        /// </summary>
        public static byte[] SHA256(this FileInfo fi)
        {
            using (var fs = File.OpenRead(fi.FullName))
                return fs.SHA256();
        }

        public static async Task<byte[]> SHA256(this DirectoryInfo di, bool excludeRootName, bool recursive, Encoding encoding = null)
        {
            var files = di.GetFiles();
            var directories = di.GetDirectories();

            files.SortByName(fullName: false);
            directories.SortByName(fullName: false);

            var fHashes = files.ForEachAsync(file => file.SHA256(encoding: encoding));

            Task<byte[][]> dHashes;

            if (recursive == true)
                dHashes = directories.ForEachAsync(
                    directory => SHA256(di: directory, excludeRootName: false, recursive: true, encoding: encoding));
            else
                dHashes = directories.ForEachAsync(
                    directory => directory.Name.ToLower().Trim(' ', '\\', '/').SHA256(encoding));

            await Task.WhenAll(fHashes, dHashes);

            var fHash = fHashes.Result.Merge().SHA256();
            var dHash = dHashes.Result.Merge().SHA256();

            if (excludeRootName == true)
                return fHash.Merge(dHash).SHA256();
            else
            {
                var dnHash = di.Name.ToLower().Trim(' ', '\\', '/').SHA256(encoding: encoding);
                return dnHash.Merge(dHash, fHash).SHA256();
            }
        }

        /// <summary>
        /// Calculates SHA256 of the file content and its name
        /// Final Hash = H(H(file).Combine(H(name/fullName)))
        /// </summary>
        public static byte[] SHA256(this FileInfo fi, Encoding encoding)
            => fi.SHA256().Merge(fi.Name.ToLower().Trim(' ', '\\', '/').SHA256(encoding)).SHA256();

        public static byte[] SHA256(this Stream s, int bufferSize = 65536)
        {
            using (var hm = new SHA256Managed())
            using (var bs = new BufferedStream(s, bufferSize))
            {
                return hm.ComputeHash(bs);
            }
        }
    }
}
