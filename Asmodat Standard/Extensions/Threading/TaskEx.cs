using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class TaskEx
    {
        public static void Await(this Task t)
            => Task.WaitAll(t);

        public static void WaitAll(this IEnumerable<Task> tasks)
            => Task.WaitAll(tasks.ToArray());
    }
}
