using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

namespace AsmodatStandard.Threading
{
    public static class TaskHelper
    {
        public static async Task<O[]> ForEachAsync<I, O>(
            this IEnumerable<I> collection, Func<I, O> func,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tb = new TransformBlock<(int index, I input), (int index, O output, Exception exception)>(i =>
                {
                    try
                    {
                        return (i.index, func(i.input), null);
                    }
                    catch (Exception ex)
                    {
                        return (i.index, default(O), ex);
                    }
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                    EnsureOrdered = false,
                    CancellationToken = cancellationToken
                });

            var items = collection.ToArray();
            for (int i = 0; i < items.Length; i++)
                await tb.SendAsync((i, items[i]));

            tb.Complete();

            var exceptions = new List<Exception>();
            var results = new O[items.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var result = await tb.ReceiveAsync();
                results[result.index] = result.output;
                if (result.exception != null)
                    exceptions.Add(result.exception);
            }

            if (exceptions.Count > 0)
                throw new AggregateException("TaskHelper Execution Failed During ForEachAsync.", exceptions);

            return results;
        }

        public static async Task<O[]> ForEachAsync<I, O>(
            this IEnumerable<I> collection, Func<I, Task<O>> func,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tb = new TransformBlock<(int index, I input), (int index, O output, Exception exception)>(
                async i =>
                {
                    try
                    {
                        return (i.index, await func(i.input), null);
                    }
                    catch (Exception ex)
                    {
                        return (i.index, default(O), ex);
                    }
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                    EnsureOrdered = false,
                    CancellationToken = cancellationToken
                });

            var items = collection.ToArray();
            for (int i = 0; i < items.Length; i++)
                await tb.SendAsync((i, items[i]));

            tb.Complete();

            var exceptions = new List<Exception>();
            var results = new O[items.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var result = await tb.ReceiveAsync();
                results[result.index] = result.output;
                if (result.exception != null)
                    exceptions.Add(result.exception);
            }

            if (exceptions.Count > 0)
                throw new AggregateException("TaskHelper Execution Failed During ForEachAsync.", exceptions);

            return results;
        }

        public static async Task ForEachAsync<I>(
            this IEnumerable<I> collection, Func<I, Task> func,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tb = new TransformBlock<I, Exception>(
                async i =>
                {
                    try
                    {
                        await func(i);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                    EnsureOrdered = false,
                    CancellationToken = cancellationToken
                });

            var items = collection.ToArray();
            foreach(var item in items)
                await tb.SendAsync(item);

            tb.Complete();

            var exceptions = new List<Exception>();
            for (int i = 0; i < items.Count(); i++)
            {
                var result = await tb.ReceiveAsync();
                if (result != null)
                    exceptions.Add(result);
            }

            if (exceptions.Count > 0)
                throw new AggregateException("TaskHelper Execution Failed During ForEachAsync.", exceptions);
        }

        public static Task<O[]> ForAsync<O>(
            int from, int to, Func<int, Task<O>> func,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
            CancellationToken cancellationToken = default(CancellationToken))
            => Enumerable.Range(from, to - from).ForEachAsync(func, maxDegreeOfParallelism, cancellationToken);

        public static Task ForAsync(
            int from, int to, Func<int, Task> func,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
            CancellationToken cancellationToken = default(CancellationToken))
            => Enumerable.Range(from, to - from).ForEachAsync(func, maxDegreeOfParallelism, cancellationToken);
    }
}