using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using Asmodat.Cryptography.Bitcoin.Mnemonic;
using Asmodat.Cryptography.Bitcoin.Address;
using System.Linq;
using Asmodat.Cryptography.Bitcoin;
using Asmodat.Cryptography.Bitcoin.Common;
using AsmodatStandard.Cryptography.Bitcoin;
using AsmodatStandard.Cryptography.Cosmos;
using AsmodatStandard.Extensions.Collections;
using System;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class Bip44Test
    {
        [Test]
        public void TestPath44()
        {
            const string ExpectedPubKey = "xpub6EncrYPyfQEpEmcMsHEnAXFQY5JkfMcyEWjExM8ppTtJ2y2TR7FfTjEVYxru1Ry9cYdsSkiPjZhv94KyJE8JT8bQuFKHpGzNV4cMscWeRLT";
            const string ExpectedPrvKey = "xprvA1oGT2s5q2gX2HXtmFhmoPJfz3UGFtu7sHoe9xjDG8MKAAhJsZwQuvv1hgSHsCV1CDzL6pq9cFPoWgh7pV4TNKwdzCKBvm6MX5YqaoPZWDu";
            const string Mnemonic = "thrive empower soon push mountain jeans chimney top jelly sorry beef hard napkin mule matrix today draft high vacuum exercise blind kitchen inflict abstract";

            var bip39 = new Bip39(Mnemonic);
            var usersMasterSeed = bip39.SeedBytes;

            var prvkey = ExtendedKey.Create(ExtendedKey.Bitcoinseed, usersMasterSeed);
            var keyPath = ExtendedKeyPathBip44.CreateBip44(0).AddChild(0);

            var derrived = keyPath.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));

            var prv = derrived.Serialize();
            var pub = derrived.GetPublicKey().Serialize();

            Assert.AreEqual(ExpectedPubKey, pub);
            Assert.AreEqual(ExpectedPrvKey, prv);
        }

        [Test]
        public void TestCosmosPath44()
        {
            var cosmosAddr = "cosmos1amfwp3cv3u3tg2h33tafpsgn4ullup5thd9cfl";
            var cosmosPubAddr = "cosmospub1addwnpepq2t9htk89a4mgjhn2hqaaglvdt3cjjkxs7y4eqw0rr38m6mfgzf35pllekn";
            var hrp = "cosmos";
            uint coinIndex = 118; //ATOM
            var pubkey = "ApZbrscva7RK81XB3qPsauOJSsaHiVyBzxjifetpQJMa";
            var Mnemonic = "nice glimpse economy runway prevent rose truck nature hole flower shift length glove hamster swim hockey betray gadget bone super planet turn tenant rather";

            var bip39 = new Bip39(Mnemonic);
            var usersMasterSeed = bip39.SeedBytes;

            var prvkey = ExtendedKey.Create(ExtendedKey.Bitcoinseed, usersMasterSeed);
            var bipPath = new BIP44Path($"m/44'118'/0'/0/0");
            var keyPath2 = ExtendedKeyPathBip44.ParseBip44(bipPath.ToString());//.AddChild(0);
            var keyPath =  ExtendedKeyPathBip44.CreateBip44(coinIndex).AddChild(0);
            var derrived = keyPath.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));
            var derrived2 = keyPath2.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));
            //var derrived2 = prvkey.GetChild(bipPath.address_index); keyPath2.Items.Aggregate( //.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));

            var bitcoinKey = derrived.GetKey(0);
            var bitcoinKey2 = derrived2.GetKey(0);
            var bitcoinAddress = bitcoinKey.PublicKey.ToAddress(new CoinParameters() { }).ToString();
            var bitcoinAddress2 = bitcoinKey2.PublicKey.ToAddress(new CoinParameters() { }).ToString();
            var recoveredCosmosAddress = Bech32.Encode(hrp, bitcoinKey.Hash160);
            var pub_key = bitcoinKey.PublicKey.Bytes.ToBase64String();
            var pub_key2 = bitcoinKey2.PublicKey.Bytes.ToBase64String();

            Assert.AreEqual(recoveredCosmosAddress, cosmosAddr);
            Assert.AreEqual(Bech32.Decode(recoveredCosmosAddress, out var prefix), bitcoinKey.Hash160);
            Assert.AreEqual(prefix, hrp);
            Assert.AreEqual(pub_key, pubkey);

            var keyPubAmino = Amino.Encode(Amino.TendermintKeyType.PubKeySecp256k1, bitcoinKey.PublicKey.Bytes);
            var recoveredCosmosPubAddress = Bech32.Encode(hrp + "pub", keyPubAmino);
            Assert.AreEqual(recoveredCosmosPubAddress, cosmosPubAddr);
        }

    }
}
