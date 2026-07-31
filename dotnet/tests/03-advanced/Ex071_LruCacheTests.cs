using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex071_LruCacheTests
{
    [Fact]
    public void EvictsLeastRecentlyUsed()
    {
        var cache = new LruCache<string, int>(2);
        cache.Put("a", 1);
        cache.Put("b", 2);
        Assert.True(cache.TryGet("a", out var a));   // touch 'a'
        Assert.Equal(1, a);
        cache.Put("c", 3);                             // evicts 'b'
        Assert.False(cache.TryGet("b", out _));
        Assert.True(cache.TryGet("a", out _));
        Assert.True(cache.TryGet("c", out _));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void UpdateRefreshesRecency()
    {
        var cache = new LruCache<string, int>(2);
        cache.Put("a", 1);
        cache.Put("b", 2);
        cache.Put("a", 10);   // refresh 'a'
        cache.Put("c", 3);    // evicts 'b'
        Assert.False(cache.TryGet("b", out _));
        Assert.True(cache.TryGet("a", out var a));
        Assert.Equal(10, a);
    }

    [Fact]
    public void RejectsNonPositiveCapacity()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<int, int>(0));
}
