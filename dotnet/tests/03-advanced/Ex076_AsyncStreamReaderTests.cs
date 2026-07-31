using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex076_AsyncStreamReaderTests
{
    [Fact]
    public async Task ConsumerCollectsExactSequenceInOrder()
    {
        var source = new[] { 10, 20, 30, 40 };
        var results = new List<int>();

        await foreach (var item in AsyncStreamReader.ReadAsync(source, TimeSpan.FromMilliseconds(1)))
        {
            results.Add(item);
        }

        Assert.Equal(new[] { 10, 20, 30, 40 }, results);
    }

    [Fact]
    public async Task ZeroDelayStillYieldsEveryItemOnce()
    {
        var source = new[] { "a", "b", "c" };
        var results = new List<string>();

        await foreach (var item in AsyncStreamReader.ReadAsync(source, TimeSpan.Zero))
        {
            results.Add(item);
        }

        Assert.Equal(new[] { "a", "b", "c" }, results);
    }

    [Fact]
    public async Task CancellationStopsEnumerationBeforeCompletion()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        using var cts = new CancellationTokenSource();
        var results = new List<int>();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var item in AsyncStreamReader.ReadAsync(source, TimeSpan.FromMilliseconds(5), cts.Token))
            {
                results.Add(item);
                if (results.Count == 2)
                    cts.Cancel();
            }
        });

        Assert.Equal(new[] { 1, 2 }, results);
    }

    [Fact]
    public async Task EmptySourceYieldsNoItems()
    {
        var results = new List<int>();

        await foreach (var item in AsyncStreamReader.ReadAsync(Array.Empty<int>(), TimeSpan.FromMilliseconds(1)))
        {
            results.Add(item);
        }

        Assert.Empty(results);
    }
}
