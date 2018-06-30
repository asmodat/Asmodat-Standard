using AsmodatStandard.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class ZipHelperTest
    {
        private string TempPath = Path.Combine(Directory.GetCurrentDirectory(), "ZipHelperTest");

        [SetUp]
        public void SetUp()
            => Directory.CreateDirectory(TempPath);

        [Test]
        public void ZipUnzipTest()
        {
            var fut = Path.Combine(TempPath, $"{Guid.NewGuid()}.zip");
            ZipHelper.CreateEmpty(fut);

            var content = Guid.NewGuid().ToString();
            ZipHelper.UpdateText(fut, "test.txt", content);

            var result = ZipHelper.ReadText(fut, "test.txt");
            Assert.AreEqual(content, result);
        }

        [Test]
        public void ParralelZipUnzipTest()
        {
            var fut = Path.Combine(TempPath, $"{Guid.NewGuid()}.zip");
            ZipHelper.CreateEmpty(fut);

            var content = new List<string>();
            for (int i = 0; i < 100; i++)
                content.Add(Guid.NewGuid().ToString());

            content.ForEach(guid => ZipHelper.UpdateText(fut, $"{guid}.txt", guid));

            Parallel.ForEach(content, guid => Assert.AreEqual(ZipHelper.ReadText(fut, $"{guid}.txt"), guid));
        }

        [TearDown]
        public void TearDown()
            => Directory.Delete(TempPath, true);
    }
}
