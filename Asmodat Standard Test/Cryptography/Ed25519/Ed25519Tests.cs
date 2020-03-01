using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Asmodat.Cryptography.Bitcoin.Mnemonic;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Cryptography;
using AsmodatStandard.Extensions.Collections;
using NUnit.Framework;
using AsmodatStandard.Cryptography.Bitcoin;
using AsmodatStandard.Cryptography;

namespace Chaos.NaCl.Tests
{
    [TestFixture]
    public class Ed25519Tests
    {
        [Test]
        public void TendermintKMSKeyPairFromSeed()
        {
            var bip39 = new Bip39("autumn valve banner happy sentence scan supreme major barrel brief snack toddler dizzy bronze science bunker trust wait dinosaur upper reward fruit bottom royal");
            var derivedKey = Ed25519HdKeyGen.DerivePath("m/44'/118'/0'/0'/0'", bip39.SeedBytes);
            var pubKey = Ed25519HdKeyGen.GetPublicKey(derivedKey.Key, withZeroByte: false);

            var pub_key = pubKey.ToBase64String();
            var priv_key = derivedKey.Key.Merge(pubKey).ToBase64String();

            if (pub_key != "SffHCJFOHOPhwJc0H89vBo5rA+OBCt+k5DvvZlVPgKo=" ||
               priv_key != "UvgD8Xm05S8doM9OM0U4eV9cul7wubQMhqvPTzxyp0pJ98cIkU4c4+HAlzQfz28GjmsD44EK36TkO+9mVU+Aqg==")
                throw new Exception("Key derivation failed miserably");
        }
    }
}