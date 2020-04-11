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

namespace AsmodatStandardTest.Ed25519.Ed25519Tests
{
    [TestFixture]
    public class Ed25519Tests
    {
        [Test]
        public void TendermintKMSKeyPairFromSeed()
        {
            var bip39 = new Bip39("autumn valve banner happy sentence scan supreme major barrel brief snack toddler dizzy bronze science bunker trust wait dinosaur upper reward fruit bottom royal");

            Assert.AreEqual( //check seed
                "49b60b942fc93142588c2c46a33f358bd51533ab2891bdd45b3788ff5b02bc79ea494db8b0ea09aefe9c9f10009e264708714f8c1b99d1d68d3e76d85b4ff0fe",
                bip39.SeedBytes.ToHexString());
            
            var derivedKey = Ed25519HdKeyGen.DerivePathCosmosTM(bip39.SeedBytes);
            var pubKey = Ed25519HdKeyGen.GetPublicKey(derivedKey.Key, withZeroByte: false);

            
            var expectedPrivKey = "";
            var expectedPrivKeyArr = expectedPrivKey.FromBase64String();

            var pub_key = pubKey.ToBase64String();
            var priv_key = derivedKey.Key.ToBase64String();

            //TODO: verify against: https://github.com/KiraCore/KB/blob/master/Cosmos/Scripts/tmkms-softsign-recovery-from-seed.py
            if (priv_key != "cMTcOhijW+6FOjbhH9KbDH1jFls2zaoxhJ1PS+Y7/UE=")
                throw new Exception("Key derivation failed miserably");
        }
    }
}