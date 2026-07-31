using System.Collections.Concurrent;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex047_ParallelTaskAggregatorTests
{
    [Fact]
    public async Task SumAsync_RunsConcurrently_AndReturnsCorrectSum()
    {
        var log = new ConcurrentQueue<string>();

        Func<Task<int>> Slow(int value, int delayMs, string name) => async () =>
        {
            log.Enqueue($"{name}-start");
            await Task.Delay(delayMs);
            log.Enqueue($"{name}-end");
            return value;
        };

        var operations = new[]
        {
            Slow(10, 60, "a"),
            Slow(20, 30, "b"),
            Slow(30, 10, "c"),
        };

        var result = await ParallelTaskAggregator.SumAsync(operations);

        Assert.Equal(60, result);

        // All three operations must have started before any (necessarily) finished,
        // proving they ran concurrently rather than sequentially awaited one by one.
        var startedCount = log.Count(entry => entry.EndsWith("-start"));
        var firstEndIndex = log.ToList().FindIndex(entry => entry.EndsWith("-end"));
        Assert.Equal(3, startedCount);
        Assert.True(firstEndIndex >= 2, "Expected all operations to have started before the first one completed.");
    }

    [Fact]
    public async Task SumAsync_EmptyOperations_ReturnsZero()
    {
        var result = await ParallelTaskAggregator.SumAsync(Array.Empty<Func<Task<int>>>());

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SumAsync_SingleOperation_ReturnsItsValue()
    {
        var operations = new Func<Task<int>>[]
        {
            () => Task.FromResult(42),
        };

        var result = await ParallelTaskAggregator.SumAsync(operations);

        Assert.Equal(42, result);
    }
}
