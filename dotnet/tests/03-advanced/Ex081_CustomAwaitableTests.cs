using System;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex081_CustomAwaitableTests
{
    [Fact]
    public async Task AwaitYieldsExpectedResult()
    {
        var awaitable = new CustomAwaitable(42);
        awaitable.Complete();

        var result = await awaitable;

        Assert.Equal(42, result);
    }

    [Fact]
    public void AwaiterIsNotCompletedUntilCompleteIsCalled()
    {
        var awaitable = new CustomAwaitable(7);
        var awaiter = awaitable.GetAwaiter();

        Assert.False(awaiter.IsCompleted);

        awaitable.Complete();

        Assert.True(awaiter.IsCompleted);
        Assert.Equal(7, awaiter.GetResult());
    }

    [Fact]
    public void OnCompletedInvokesContinuationOnceCompleted()
    {
        var awaitable = new CustomAwaitable(99);
        var awaiter = awaitable.GetAwaiter();
        var invoked = false;

        awaiter.OnCompleted(() => invoked = true);
        Assert.False(invoked);

        awaitable.Complete();

        Assert.True(invoked);
    }

    [Fact]
    public void OnCompletedInvokesContinuationImmediatelyWhenAlreadyCompleted()
    {
        var awaitable = new CustomAwaitable(3);
        awaitable.Complete();
        var awaiter = awaitable.GetAwaiter();
        var invoked = false;

        awaiter.OnCompleted(() => invoked = true);

        Assert.True(invoked);
    }

    [Fact]
    public void GetResultThrowsBeforeCompletion()
    {
        var awaitable = new CustomAwaitable(1);
        var awaiter = awaitable.GetAwaiter();

        Assert.Throws<InvalidOperationException>(() => awaiter.GetResult());
    }
}
