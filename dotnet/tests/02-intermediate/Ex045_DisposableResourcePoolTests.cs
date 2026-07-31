using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex045_DisposableResourcePoolTests
{
    [Fact]
    public void UsingBlock_DisposesAutomatically()
    {
        DisposableResourcePool pool;
        using (pool = new DisposableResourcePool(2))
        {
            Assert.False(pool.IsDisposed);
        }

        Assert.True(pool.IsDisposed);
        Assert.Equal(1, pool.DisposeCallCount);
    }

    [Fact]
    public void Acquire_ReturnsSequentialHandlesAndTracksAvailability()
    {
        using var pool = new DisposableResourcePool(3);

        Assert.Equal(3, pool.AvailableCount);

        var first = pool.Acquire();
        var second = pool.Acquire();

        Assert.Equal(0, first);
        Assert.Equal(1, second);
        Assert.Equal(1, pool.AvailableCount);
    }

    [Fact]
    public void Acquire_ThrowsWhenPoolExhausted()
    {
        using var pool = new DisposableResourcePool(1);
        pool.Acquire();

        Assert.Throws<InvalidOperationException>(() => pool.Acquire());
    }

    [Fact]
    public void Release_FreesHandleForReuse()
    {
        using var pool = new DisposableResourcePool(1);
        var handle = pool.Acquire();
        pool.Release(handle);

        Assert.Equal(1, pool.AvailableCount);
        Assert.Equal(0, pool.Acquire());
    }

    [Fact]
    public void Release_ThrowsWhenHandleNotAcquired()
    {
        using var pool = new DisposableResourcePool(2);

        Assert.Throws<InvalidOperationException>(() => pool.Release(0));
    }

    [Fact]
    public void Release_ThrowsForOutOfRangeHandle()
    {
        using var pool = new DisposableResourcePool(2);

        Assert.Throws<ArgumentOutOfRangeException>(() => pool.Release(5));
    }

    [Fact]
    public void Acquire_ThrowsObjectDisposedExceptionAfterDispose()
    {
        var pool = new DisposableResourcePool(2);
        pool.Dispose();

        Assert.Throws<ObjectDisposedException>(() => pool.Acquire());
    }

    [Fact]
    public void Dispose_CalledTwice_IsIdempotentButCountsEachCall()
    {
        var pool = new DisposableResourcePool(2);

        pool.Dispose();
        pool.Dispose();

        Assert.True(pool.IsDisposed);
        Assert.Equal(2, pool.DisposeCallCount);
    }

    [Fact]
    public void Constructor_ThrowsForNonPositiveCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisposableResourcePool(0));
    }
}
