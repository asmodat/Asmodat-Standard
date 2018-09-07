using AsmodatStandard.IO;
using NUnit.Framework;
using AsmodatStandard.Extensions.IO;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.IO
{
    [TestFixture]
    public class DirectoryInfoExTest
    {
        [Test]
        public void HasSubDirectory()
        {
            Assert.IsTrue(
                @"C:\ble\".ToDirectoryInfo().HasSubDirectory(
                @"C:\ble".ToDirectoryInfo()));

            Assert.IsTrue(
                @"C:\".ToDirectoryInfo().HasSubDirectory(
                @"C:\".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\".ToDirectoryInfo().HasSubDirectory(
                @"D:\".ToDirectoryInfo()));

            Assert.IsTrue(
                @"C:".ToDirectoryInfo().HasSubDirectory(
                @"C:".ToDirectoryInfo()));

            Assert.IsFalse(
               @"C:".ToDirectoryInfo().HasSubDirectory(
               @"D:".ToDirectoryInfo()));

            Assert.IsTrue(
                @"D".ToDirectoryInfo().HasSubDirectory(
                @"D".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C".ToDirectoryInfo().HasSubDirectory(
                @"D".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\ble\".ToDirectoryInfo().HasSubDirectory(
                @"C:\ble\a".ToDirectoryInfo()));

            Assert.IsTrue(
                @"C:\ble\bla\".ToDirectoryInfo().HasSubDirectory(
                @"C:\ble".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\ble\bla\".ToDirectoryInfo().HasSubDirectory(
                @"D:\ble".ToDirectoryInfo()));

            Assert.IsFalse(
                @"C:\ble\bla".ToDirectoryInfo().HasSubDirectory(
                @"C:\ble\ble".ToDirectoryInfo()));
        }
    }
}
