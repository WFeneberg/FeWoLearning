using System.Collections.Concurrent;
using System.Threading;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 073 — Parallel array sum (reference solution).
// Parallel.ForEach with a range partitioner: each task keeps a thread-local
// long accumulator and folds it into the shared total via Interlocked.Add
// on completion, avoiding contention on every element.
public static class ParallelSum
{
    public static long Sum(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length == 0)
            return 0L;

        long total = 0;

        Parallel.ForEach(
            Partitioner.Create(0, values.Length),
            () => 0L,
            (range, _, localSum) =>
            {
                for (var i = range.Item1; i < range.Item2; i++)
                    localSum += values[i];
                return localSum;
            },
            localSum => Interlocked.Add(ref total, localSum));

        return total;
    }
}
