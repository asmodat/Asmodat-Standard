using System;
using Chaos.NaCl.Internal.Ed25519Ref10;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Cryptography
{
    public class HdKey
    {
        public byte[] Key { get; internal set; }
        public byte[] ChainCode { get; internal set; }
    }

    public static class Ed25519HdKeyGen
    {
        private const string PathRegex = "^m(\\/[0-9]+')+$";
        private const string Ed25519Curve = "ed25519 seed";
        private const long HardenedOffset = 0x80000000;

        public static HdKey GetMasterKeyFromSeed(ReadOnlySpan<byte> seed)
        {
            var key = Encoding.UTF8.GetBytes(Ed25519Curve).ToList();

            using (var hash = new HMACSHA512(key.ToArray()))
            {
                hash.Initialize();
                hash.ComputeHash(seed.ToArray());
                var i = hash.Hash.AsSpan();

                return new HdKey
                {
                    Key = i.Slice(0, 32).ToArray(),
                    ChainCode = i.Slice(32).ToArray()
                };
            }
        }

        public static byte[] GetPublicKey(byte[] privateKey, bool withZeroByte = true)
        {
            Chaos.NaCl.Ed25519.KeyPairFromSeed(out var publicKey, out _, privateKey);

            if (withZeroByte)
            {
                var pub = publicKey.ToList();
                pub.Insert(0, 0x00);
                return pub.ToArray();
            }

            return publicKey;
        }

        public static bool IsValidPath(string path)
        {
            return Regex.IsMatch(path, PathRegex);
        }

        private static HdKey Derive(HdKey parent, UInt32 index)
        {
            var data = parent.Key.ToList();
            var indexBuffer = BitConverter.GetBytes(index);
            indexBuffer = indexBuffer.Reverse().ToArray();

            data.AddRange(indexBuffer);
            data.Insert(0, 0);

            using (var hash = new HMACSHA512(parent.ChainCode))
            {
                var i = hash.ComputeHash(data.ToArray()).AsSpan();

                return new HdKey
                {
                    Key = i.Slice(0, 32).ToArray(),
                    ChainCode = i.Slice(32).ToArray()
                };
            }
        }

        public static HdKey DerivePath(string path, ReadOnlySpan<byte> seed)
        {
            if (!IsValidPath(path))
            {
                throw new ArgumentException("Path is not valid");
            }

            var key = GetMasterKeyFromSeed(seed);

            var segments = path.Split('/').AsSpan().Slice(1).ToArray();
            var intSegments = new List<int>();

            foreach (var segment in segments)
            {
                var nSegment = segment.Replace("'", "");
                intSegments.Add(Convert.ToInt32(nSegment));
            }

            var parentKey = key;
            foreach (var s in intSegments)
            {
                parentKey = Derive(parentKey, (UInt32)(s + HardenedOffset));
            }

            return parentKey;
        }

        public static byte[] FromHex(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ToHex(this byte[] data)
        {
            if (data == null)
            {
                return String.Empty;
            }

            string hex = BitConverter.ToString(data);
            return hex.Replace("-", "").ToLower();
        }
    }
}