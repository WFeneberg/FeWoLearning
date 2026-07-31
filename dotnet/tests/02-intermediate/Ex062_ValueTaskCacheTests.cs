using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex062_ValueTaskCacheTests
{
    [Fact]
    public async Task GetAsync_CacheMiss_CompletesAsynchronously_AndReturnsComputedValue()
    {
        var callCount = 0;
        var cache = new ValueTaskCache(key =>
        {
            callCount++;
            return key * key;
        });

        var missTask = cache.GetAsync(7);

        // A cache miss must not complete synchronously: it has to go through
        // the asynchronous compute path before a result is available.
        Assert.False(missTask.IsCompleted);

        var missResult = await missTask;

        Assert.Equal(49, missResult);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task GetAsync_CacheHit_CompletesSynchronously_AndReturnsCachedValue()
    {
        var callCount = 0;
        var cache = new ValueTaskCache(key =>
        {
            callCount++;
            return key * key;
        });

        var seedResult = await cache.GetAsync(3);
        Assert.Equal(9, seedResult);
        Assert.Equal(1, callCount);

        var hitTask = cache.GetAsync(3);

        // A cache hit must complete synchronously: no awaiting, no re-computation.
        Assert.True(hitTask.IsCompletedSuccessfully);

        var hitResult = await hitTask;

        Assert.Equal(9, hitResult);
        Assert.Equal(1, callCount);
    }
}
