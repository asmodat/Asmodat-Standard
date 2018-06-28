using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using AsmodatStandard.Threading;

namespace AsmodatStandard.Cryptography
{
    public static partial class HashHelper
    {
        public static byte[] MD5(this string str, Encoding encoding = null)
            => str.ToByteArray(encoding ?? Encoding.UTF8).MD5();

        public static byte[] MD5(this byte[] ba)
        {
            using (var hm = MD5CryptoServiceProvider.Create())
                return hm.ComputeHash(ba);
        }

        /// <summary>
        /// Calculates MD5 of the file content
        /// </summary>
        public static byte[] MD5(this FileInfo fi)
        {
            using (var fs = File.OpenRead(fi.FullName))
                return fs.MD5();
        }

        public static async Task<byte[]> MD5(this DirectoryInfo di, bool excludeRootName, bool recursive, Encoding encoding = null)
        {
            var files = di.GetFiles();
            var directories = di.GetDirectories();

            files.SortByName(fullName: false);
            directories.SortByName(fullName: false);

            var fHashes = files.ForEachAsync(file => file.MD5(encoding: encoding));

            Task<byte[][]> dHashes;

            if (recursive == true)
                dHashes = directories.ForEachAsync(
                    directory => MD5(di: directory, excludeRootName: false, recursive: true, encoding: encoding));
            else
                dHashes = directories.ForEachAsync(
                    directory => directory.Name.ToLower().Trim(' ','\\','/').MD5(encoding));

            await Task.WhenAll(fHashes, dHashes);

            var fHash = fHashes.Result.Merge().MD5();
            var dHash = dHashes.Result.Merge().MD5();

            if (excludeRootName == true)
                return fHash.Merge(dHash).MD5();
            else
            {
                var dnHash = di.Name.ToLower().Trim(' ', '\\', '/').MD5(encoding: encoding);
                return dnHash.Merge(dHash, fHash).MD5();
            }
        }

        /// <summary>
        /// Calculates MD5 of the file content and its name
        /// Final Hash = H(H(file).Combine(H(name/fullName)))
        /// </summary>
        public static byte[] MD5(this FileInfo fi, Encoding encoding)
            => fi.MD5().Merge(fi.Name.ToLower().Trim(' ', '\\', '/').MD5(encoding)).MD5();

        public static byte[] MD5(this Stream s, int bufferSize = 65536)
        {
            using (var hm = MD5CryptoServiceProvider.Create())
            using (var bs = new BufferedStream(s, bufferSize))
            {
                return hm.ComputeHash(bs);
            }
        }
    }
}
