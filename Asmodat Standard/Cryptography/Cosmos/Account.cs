using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Asmodat.Cryptography.Bitcoin.Address;
using Asmodat.Cryptography.Bitcoin.Common;
using Asmodat.Cryptography.Bitcoin.Mnemonic;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Cryptography.Bitcoin;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace AsmodatStandard.Cryptography.Cosmos
{
    public class Account
    {
        public string Prefix { get; private set; }
        //public uint CoinIndex { get; private set; }
        public BIP44Path Path { get; private set; }
        public string BitcoinAddress { get; private set; }
        public string CosmosAddress { get; private set; }
        public string CosmosPubAddress { get; private set; }

        /// <summary>
        /// CosmosAddress Data
        /// </summary>
        public byte[] Data { get; private set; }

        private ExtendedKey _extendedKey = null;
        private BitcoinKey _bitcoinKey = null;
        private Key _key { get; set; }

        public string PublicKey { get; private set; }

        /*
                /// <summary>
                /// 
                /// </summary>
                /// <param name="prefix"></param>
                /// <param name="coinIndex">https://github.com/satoshilabs/slips/blob/master/slip-0044.md</param>
                public Account(
                    string prefix,
                    uint coinIndex)
                {
                    this.Prefix = prefix;
                    this.CoinIndex = coinIndex;
                }*/

        public Account(
            string prefix,
            string path)
        {
            this.Prefix = prefix;
            this.Path = new BIP44Path(path);
        }

        public void InitializeWithMnemonic(string mnemonic)
        {
            var bip39 = new Bip39(mnemonic);
            var usersMasterSeed = bip39.SeedBytes;
            ExtKey extKey = new ExtKey(usersMasterSeed);
            ExtKey derived = extKey.Derive(KeyPath.Parse(this.Path.ToSlimString()));
            _key = derived.PrivateKey;
            this.PublicKey = _key.PubKey.ToBytes().ToBase64String();

            var prvkey = ExtendedKey.Create(ExtendedKey.Bitcoinseed, usersMasterSeed);
            var keyPath = ExtendedKeyPathBip44.CreateBip44(this.Path.coin_type, accountIndex: this.Path.address_index).AddChild(0);
            _extendedKey = keyPath.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));
            _bitcoinKey = _extendedKey.GetKey(0);

            this.BitcoinAddress = _bitcoinKey.PublicKey.ToAddress(new CoinParameters() { }).ToString();
            this.Data = _bitcoinKey.Hash160;
            this.CosmosAddress = Bech32.Encode(this.Prefix, this.Data);

            var keyPubAmino = Amino.Encode(Amino.TendermintKeyType.PubKeySecp256k1, _bitcoinKey.PublicKey.Bytes);
            this.CosmosPubAddress = Bech32.Encode(this.Prefix + "pub", keyPubAmino);
        }

        public byte[] Sign(byte[] bytes, out byte[] hash)
        {
            hash = bytes.SHA256();
            var signature = _bitcoinKey.PrivateKey.Key.Sign(hash);
            var signatureBytes = new byte[64];
            signature.R.ToByteArrayUnsigned().CopyTo(signatureBytes, 0);
            signature.S.ToByteArrayUnsigned().CopyTo(signatureBytes, 32);
            return signatureBytes;
        }

        public byte[] Sign2(byte[] msg)
        {
            byte[] hash;
            using (SHA256 sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(msg);
            }

            var uint256le = new uint256(hash, true);
            var signature = _key.Sign(uint256le, false);

            byte[] signatureBytes = new byte[64];
            signature.R.ToByteArrayUnsigned().CopyTo(signatureBytes, 0);
            signature.S.ToByteArrayUnsigned().CopyTo(signatureBytes, 32);
            return signatureBytes;
        }
    }
}
