﻿using NUnit.Framework;

namespace AsmodatStandard.Extensions.StringExTests
{
    [TestFixture]
    public class StringExTest
    {
        [Test]
        public void GZipTest()
        {
            void Test(string s) 
                => Assert.AreEqual(s, s.GZip().UnGZip());

            Test("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            Test("babacc");
            Test("babagggggggggggggggggggghhhhhhhhhhhhhhhcc");
            Test("abcdefghijkabcdefghijkabcdefghijkabcdefghijkab0987654");
        }
    }
}