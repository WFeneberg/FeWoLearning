using System;
using System.Linq;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex073_ParallelSumTests
{
    [Fact]
    public void MatchesSequentialSumForLargeArray()
    {
        var values = Enumerable.Range(1, 1_000_000).ToArray();
        long expected = 0;
        foreach (var v in values)
            expected += v;

        var actual = ParallelSum.Sum(values);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HandlesNegativeAndPositiveValues()
    {
        var values = Enumerable.Range(-500_000, 1_000_001).ToArray(); // -500000..500000
        long expected = values.Aggregate(0L, (acc, v) => acc + v);

        var actual = ParallelSum.Sum(values);

        Assert.Equal(0L, expected);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReturnsZeroForEmptyArray()
    {
        Assert.Equal(0L, ParallelSum.Sum(Array.Empty<int>()));
    }

    [Fact]
    public void ThrowsOnNullArray()
    {
        Assert.Throws<ArgumentNullException>(() => ParallelSum.Sum(null!));
    }

    [Fact]
    public void SumExceedsIntRangeUsesLongAccumulation()
    {
        var values = Enumerable.Repeat(int.MaxValue, 10).ToArray();
        long expected = 10L * int.MaxValue;

        var actual = ParallelSum.Sum(values);

        Assert.Equal(expected, actual);
    }
}
