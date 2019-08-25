using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class TaskEx
    {
        public static void Await(this Task t)
            => Task.WaitAll(t);

        public static T Await<T>(this Task<T> t)
            => t.Result;

        public static void WaitAll(this IEnumerable<Task> tasks)
            => Task.WaitAll(tasks.ToArray());

        public static async Task<T> Timeout<T>(
            this Task<T> task, 
            int msTimeout, 
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (task == null)
                throw new ArgumentNullException($"Task was not defined: '{memberName ?? "unknown"}'");

            using (var cts = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(msTimeout, cts.Token));
                if (completedTask == task)
                {
                    if (task.Exception != null || task.IsFaulted)
                        throw new ArgumentException($"Method '{memberName ?? "unknown"}' failed before Timeout occured.\n\rSource: '{sourceFilePath ?? "unknown"}:{sourceLineNumber}'", task?.Exception);

                    cts.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException($"Task timed out: '{memberName ?? "unknown"}'");
                }
            }
        }

        public static async Task<T> TryCancelAfter<T>(this Task<T> task, CancellationToken ct, int msTimeout)
        {
            if (ct == null || ct.IsCancellationRequested)
                return await task;

            using (var ctsTimeout = new CancellationTokenSource())
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

                var completedTask = await Task.WhenAny(task, Task.Delay(msTimeout, ctsTimeout.Token));
                if (completedTask != task)
                    cts.Cancel();

                return await task;
            }
        }
    }
}
