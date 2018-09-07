using AsmodatStandard.IO;
using NUnit.Framework;
using AsmodatStandard.Extensions.IO;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.IO
{
    [TestFixture]
    public class FileInfoExTest
    {
        [Test]
        public void HasSubDirectory()
        {
            Assert.IsTrue(
                @"C:\ble\a.txt".ToFileInfo().HasSubDirectory(
                @"C:\ble".ToDirectoryInfo()));

            Assert.IsTrue(
                @"C:\a.txt".ToFileInfo().HasSubDirectory(
                @"C:\".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\a.txt".ToFileInfo().HasSubDirectory(
                @"D:\".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\ble\a.txt".ToFileInfo().HasSubDirectory(
                @"C:\ble\a".ToDirectoryInfo()));

            Assert.IsTrue(
                @"C:\ble\bla\a.txt".ToFileInfo().HasSubDirectory(
                @"C:\ble".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\ble\bla\a.txt".ToFileInfo().HasSubDirectory(
                @"C:\ble\ble".ToDirectoryInfo()));
        }
    }
}
