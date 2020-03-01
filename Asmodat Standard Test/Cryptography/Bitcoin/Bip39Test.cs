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
using AsmodatStandard.Extensions.Cryptography;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class Bip39Test
    {
        [Test]
        public void RandomMnemonicTest()
        {
            for(int i = 1; i < 128; i++)
            {
                if (i < 12)
                    Assert.Throws<ArgumentException>(() => { BitcoinEx.RandomBip39Mnemonic(i); });
                else

                    if (i % 3 == 0 && i % 2 == 0)
                        Assert.AreEqual(BitcoinEx.RandomBip39Mnemonic(i).Split().Length, i);
                    else
                        Assert.Throws<Exception>(() => { BitcoinEx.RandomBip39Mnemonic(i); });
            }
        }



    }
}
