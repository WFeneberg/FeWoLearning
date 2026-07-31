using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex004_ArrayStatisticsTests
{
    private static readonly int[] Sample = { 4, -7, 12, 0, -3, 9, -1 };

    [Fact]
    public void Min_ReturnsSmallestValue()
        => Assert.Equal(-7, ArrayStatistics.Min(Sample));

    [Fact]
    public void Max_ReturnsLargestValue()
        => Assert.Equal(12, ArrayStatistics.Max(Sample));

    [Fact]
    public void Sum_ReturnsTotalOfAllValues()
        => Assert.Equal(14, ArrayStatistics.Sum(Sample));

    [Fact]
    public void SingleElementArray_AllStatsEqualThatElement()
    {
        var single = new[] { 42 };

        Assert.Equal(42, ArrayStatistics.Min(single));
        Assert.Equal(42, ArrayStatistics.Max(single));
        Assert.Equal(42, ArrayStatistics.Sum(single));
    }
}
