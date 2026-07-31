using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex088_ReaderWriterCacheTests
{
    [Fact]
    public void AddOrUpdate_AddsThenUpdatesExistingEntry()
    {
        var cache = new ReaderWriterCache<string, int>();

        var added = cache.AddOrUpdate("count", _ => 1, (_, current) => current + 1);
        Assert.Equal(1, added);

        var updated = cache.AddOrUpdate("count", _ => 1, (_, current) => current + 1);
        Assert.Equal(2, updated);

        Assert.True(cache.TryGet("count", out var value));
        Assert.Equal(2, value);
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Remove_DeletesEntryAndReportsWhetherItExisted()
    {
        var cache = new ReaderWriterCache<string, int>();
        cache.AddOrUpdate("a", _ => 1, (_, c) => c);

        Assert.True(cache.Remove("a"));
        Assert.False(cache.Remove("a"));
        Assert.False(cache.TryGet("a", out _));
        Assert.Equal(0, cache.Count);
    }

    // Deterministic invariant check: a single key always stores two ints
    // whose sum is 100. One writer thread repeatedly swaps the split while
    // several reader threads poll the value concurrently. If write access
    // were not exclusive, a reader could observe a torn update where A and B
    // come from two different writes and the invariant breaks. With correct
    // ReaderWriterLockSlim usage the invariant must hold on every read, no
    // matter how the threads happen to interleave.
    [Fact]
    public void ConcurrentReadsAndWritesNeverObserveATornUpdate()
    {
        var cache = new ReaderWriterCache<string, (int A, int B)>();
        cache.AddOrUpdate("pair", _ => (100, 0), (_, __) => (100, 0));

        const int writerIterations = 2_000;
        const int readerCount = 4;
        const int readsPerReader = 5_000;
        var tornReads = 0;

        var writer = Task.Run(() =>
        {
            for (var i = 0; i < writerIterations; i++)
            {
                var a = i % 101;
                var b = 100 - a;
                cache.AddOrUpdate("pair", _ => (a, b), (_, __) => (a, b));
            }
        });

        var readers = Enumerable.Range(0, readerCount)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < readsPerReader; i++)
                {
                    if (cache.TryGet("pair", out var value) && value.A + value.B != 100)
                    {
                        Interlocked.Increment(ref tornReads);
                    }
                }
            }))
            .ToArray();

        Task.WaitAll(readers.Append(writer).ToArray());

        Assert.Equal(0, tornReads);
        Assert.True(cache.TryGet("pair", out var final));
        Assert.Equal(100, final.A + final.B);
    }

    [Fact]
    public void AddOrUpdate_RejectsNullFactories()
    {
        var cache = new ReaderWriterCache<string, int>();
        Assert.Throws<ArgumentNullException>(() => cache.AddOrUpdate("x", null!, (_, c) => c));
        Assert.Throws<ArgumentNullException>(() => cache.AddOrUpdate("x", _ => 1, null!));
    }
}
