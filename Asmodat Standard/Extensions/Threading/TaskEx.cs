using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class TaskEx
    {
        public static async Task ToTask(this Action action)
            => await Task.Factory.StartNew(() => action.Invoke());
        public static async Task<T1> ToTask<T1>(this Func<T1> func)
            => await Task.Factory.StartNew(() => func.Invoke());
        public static async Task<T2> ToTask<T1, T2>(this Func<T1, T2> func, T1 input)
            => await Task.Factory.StartNew(() => func.Invoke(input));
        public static async Task<T3> ToTask<T1, T2, T3>(this Func<T1, T2, T3> func, T1 in1, T2 in2)
            => await Task.Factory.StartNew(() => func.Invoke(in1, in2));
        public static async Task<(T3, T4)> ToTask<T1, T2, T3, T4>(this Func<T1, T2, (T3, T4)> func, T1 in1, T2 in2)
            => await Task.Factory.StartNew(() => func.Invoke(in1, in2));
        public static async Task<T4> ToTask<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> func, T1 in1, T2 in2, T3 in3)
            => await Task.Factory.StartNew(() => func.Invoke(in1, in2, in3));
        public static async Task<(T4, T5)> ToTask<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, (T4, T5)> func, T1 in1, T2 in2, T3 in3)
            => await Task.Factory.StartNew(() => func.Invoke(in1, in2, in3));
        public static async Task<T5> ToTask<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> func, T1 in1, T2 in2, T3 in3, T4 in4)
            => await Task.Factory.StartNew(() => func.Invoke(in1, in2, in3, in4));


        public static void Await(this Task t)
            => Task.WaitAll(t);

        public static T Await<T>(this Task<T> t)
            => t.Result;

        public static void WaitAll(this IEnumerable<Task> tasks)
            => Task.WaitAll(tasks.ToArray());

        public static async Task Timeout(
            this Task task,
            int msTimeout,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (task == null)
                throw new ArgumentNullException($"Task was not defined: '{memberName ?? "unknown"}'");

            var sw = Stopwatch.StartNew();
            using (var cts = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(msTimeout, cts.Token));
                if (completedTask == task)
                {
                    cts.Cancel();
                    await task;  // Very important in order to propagate exceptions
                    return;
                }

                throw new TimeoutException($"Task timed out: '{memberName ?? "unknown"}' elapsed {sw.ElapsedMilliseconds}/{msTimeout} [ms].", task?.Exception);
            }
        }

        public static async Task<T> Timeout<T>(
            this Task<T> task, 
            int msTimeout, 
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (task == null)
                throw new ArgumentNullException($"Task was not defined: '{memberName ?? "unknown"}'");

            var sw = Stopwatch.StartNew();
            using (var cts = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(msTimeout, cts.Token));
                if (completedTask == task)
                {
                    cts.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }

                throw new TimeoutException($"Task timed out: '{memberName ?? "unknown"}' elapsed {sw.ElapsedMilliseconds}/{msTimeout} [ms].", task?.Exception);
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
