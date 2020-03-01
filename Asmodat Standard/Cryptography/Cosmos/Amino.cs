using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using Google.Protobuf;
namespace AsmodatStandard.Cryptography.Cosmos
{
    public class Amino
    {
        /// <summary>
        /// https://tendermint.com/docs/spec/blockchain/encoding.html#public-key-cryptography
        /// </summary>
        public enum TendermintKeyType
        {
            PubKeyEd25519 = 0,
            PubKeySecp256k1 = 1,
            PrivKeyEd25519 = 2,
            PrivKeySecp256k1 = 3,
            SignatureEd25519 = 4,
            SignatureSecp256k1 = 5
        }

        /// <summary>
        /// https://github.com/tendermint/go-amino#computing-the-prefix-and-disambiguation-bytes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encode(TendermintKeyType type, byte[] key)
        {
            var typeName = $"tendermint/{type.ToString()}";
            var bType = Amino.GetTypeIdentifiers(typeName).prefix;
            //to byte array puts most valuable byte at index 0
            var bLength = ((UInt64)key.LongLength).ToByteArray().Reverse().TrimStart(0);

            return bType.Merge(bLength).Merge(key);
        }
        
        public static  (byte[] disambiguation, byte[] prefix) GetTypeIdentifiers(string name)
        {
            var hash = name.SHA256();
            var disambiguation = new List<byte>();
            var prefix = new List<byte>();
            for (int i = 0; i < hash.Length; i++)
            {
                if (disambiguation.Count == 0 && hash[i] == 0)
                    continue;

                if (disambiguation.Count < 3)
                {
                    disambiguation.Add(hash[i]);
                    continue;
                }

                if (prefix.Count == 0 && hash[i] == 0)
                    continue;

                prefix.Add(hash[i]);

                if (prefix.Count == 4)
                    return (disambiguation.ToArray(), prefix.ToArray());
            }

            throw new Exception($"Cound not generate Amino disambiguation and prefix for the type '{name ?? "undefined"}'");
        }

        public static string GetPrefix(string name) => GetTypeIdentifiers(name).prefix.ToHexString();
        public static string GetDisambiguation(string name) => GetTypeIdentifiers(name).prefix.ToHexString();

        public static byte[] Wrap(byte[] raw, byte[] typePrefix, bool isPrefixLength)
        {
            var len = raw.Length + typePrefix.Length;
            if (isPrefixLength)
                len += CodedOutputStream.ComputeUInt64Size((ulong)len);

            var msg = new byte[len];
            var cos = new CodedOutputStream(msg);
            if (isPrefixLength)
                cos.WriteUInt64((ulong)(raw.Length + typePrefix.Length));

            for (int i = 0; i < typePrefix.Length; i++)
                cos.WriteRawTag(typePrefix[i]);
            
            for (int i = 0; i < raw.Length; i++)
                cos.WriteRawTag(raw[i]);
            
            cos.Flush();
            return msg;
        }
    }
}
