using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex089_ObjectPoolImplTests
{
    [Fact]
    public void ReturnedInstanceIsReusedByNextGet()
    {
        var pool = new ObjectPoolImpl(bufferSize: 16, initialSize: 1);

        var first = pool.Get();
        Assert.Equal(1, pool.CreatedCount);
        Assert.Equal(0, pool.AvailableCount);

        pool.Return(first);
        Assert.Equal(1, pool.AvailableCount);

        var second = pool.Get();
        Assert.Same(first, second);
        Assert.Equal(1, pool.CreatedCount); // reused, no growth needed
    }

    [Fact]
    public void PoolGrowsWhenExhausted()
    {
        var pool = new ObjectPoolImpl(bufferSize: 8, initialSize: 1);

        var a = pool.Get(); // takes the only pre-warmed instance -> CreatedCount 1
        var b = pool.Get(); // pool empty -> grows, CreatedCount 2

        Assert.Equal(2, pool.CreatedCount);
        Assert.NotSame(a, b);
        Assert.Equal(0, pool.AvailableCount);

        pool.Return(a);
        pool.Return(b);
        Assert.Equal(2, pool.AvailableCount);
    }

    [Fact]
    public void ReturningSameInstanceTwiceThrows()
    {
        var pool = new ObjectPoolImpl(bufferSize: 4, initialSize: 1);
        var buffer = pool.Get();

        pool.Return(buffer);
        Assert.Throws<InvalidOperationException>(() => pool.Return(buffer));
    }

    [Fact]
    public void RejectsNonPositiveBufferSize()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new ObjectPoolImpl(bufferSize: 0, initialSize: 1));
}
