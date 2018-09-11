using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Threading;
using System.Diagnostics;
using System;
using AsmodatStandard.Types;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Types;

namespace AsmodatStandardTest.Types
{
    [TestFixture]
    public class PreetyExceptionTest
    {
        [Test]
        public void Test()
        {
            void test()
            {
                int i = 0;
                throw new Exception("bla", new AggregateException("ble"));
            }

            try
            {
                test();
            }
            catch(Exception ex)
            {
                var pe = ex.ToPreetyException();
                var str = pe.JsonSerialize(Newtonsoft.Json.Formatting.Indented);
            }
        }

        [Test]
        public async Task TestReThrow()
        {
            async Task test()
            {
                int i = 0;
                throw new Exception("bla", new AggregateException("ble"));
            }

            try
            {
                await test().TryCatchRetryAsync(10, 1);
            }
            catch (Exception ex)
            {
                var pe = ex.ToPreetyException();
                var str = pe.JsonSerialize(Newtonsoft.Json.Formatting.Indented);
            }
        }
    }
}
