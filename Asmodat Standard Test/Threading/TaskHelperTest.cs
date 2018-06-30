using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Threading;
using System.Diagnostics;
using System;
using AsmodatStandard.Threading;

namespace AsmodatStandardTest.Threading.TaskHelperTest
{
    [TestFixture]
    public class TaskHelperTest
    {
        private static readonly int _timeout = 60000;

        [Test]
        public async Task ForEachAsyncTest()
        {
            var sw = Stopwatch.StartNew();
            var ss = new SemaphoreSlim(1, 1);

            var aut = Enumerable.Range(1, 1000).ToArray(); //1..999
            int sum = aut.Sum(); 
            int result = 0;

            var output = await TaskHelper.ForEachAsync(aut, i => {

                var start = ss.Lock(() => ++result);

                while (start < 1000 && ss.Lock(() => result) <= start) //ensure parallelism
                {
                    Thread.Sleep(10);
                    if (sw.ElapsedMilliseconds > _timeout)
                        throw new TimeoutException($"{sw.ElapsedMilliseconds}/{_timeout} [ms]");
                }

                return i;
            }, maxDegreeOfParallelism: 100);

            Assert.AreEqual(1000, result);
            Assert.True(aut.SequenceEqual(output)); //ensure order
        }

        [Test]
        public async Task ForEachAsync2Test()
        {
            var sw = Stopwatch.StartNew();
            var ss = new SemaphoreSlim(1, 1);

            var aut = Enumerable.Range(1, 1000).ToArray(); //1..999
            int sum = aut.Sum();
            int result = 0;

            var output = await TaskHelper.ForEachAsync(aut, async i => {

                var start = ss.Lock(() => ++result);

                while (start < 1000 && ss.Lock(() => result) <= start) //ensure parallelism
                {
                    await Task.Delay(10);
                    if (sw.ElapsedMilliseconds > _timeout)
                        throw new TimeoutException($"{sw.ElapsedMilliseconds}/{_timeout} [ms]");
                }

                return i;
            }, maxDegreeOfParallelism: 100);

            Assert.AreEqual(1000, result);
            Assert.True(aut.SequenceEqual(output)); //ensure order
        }
    }
}
