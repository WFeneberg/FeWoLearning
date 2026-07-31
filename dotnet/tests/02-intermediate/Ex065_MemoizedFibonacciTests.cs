using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex065_MemoizedFibonacciTests
{
    [Theory]
    [InlineData(0, 0L)]
    [InlineData(1, 1L)]
    [InlineData(2, 1L)]
    [InlineData(10, 55L)]
    [InlineData(30, 832040L)]
    [InlineData(40, 102334155L)]
    public void Calculate_ReturnsExpectedFibonacciNumber(int n, long expected)
        => Assert.Equal(expected, MemoizedFibonacci.Calculate(n));

    [Fact]
    public void Calculate_KeepsRecursiveCallCountBounded()
    {
        const int n = 40;

        MemoizedFibonacci.Calculate(n);

        // A naive (non-memoized) recursive Fibonacci makes an exponential
        // number of calls (millions for n = 40). A correctly memoized
        // implementation should never need more than a small multiple of n
        // recursive calls, since every sub-problem is solved at most once.
        Assert.True(
            MemoizedFibonacci.CallCount <= n * 3,
            $"Expected a bounded recursive call count (<= {n * 3}) for memoized recursion, " +
            $"but got {MemoizedFibonacci.CallCount}. Is the memo cache actually being used?");
    }
}
