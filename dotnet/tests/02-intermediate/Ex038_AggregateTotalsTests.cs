using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex038_AggregateTotalsTests
{
    [Fact]
    public void ComputeRunningTotals_ReturnsRunningTotalsAndFinalTotal()
    {
        var prices = new[] { 19.99m, 5.50m, 42.00m, 3.25m };

        var (runningTotals, finalTotal) = AggregateTotals.ComputeRunningTotals(prices);

        Assert.Equal(4, runningTotals.Count);
        Assert.Equal(19.99m, runningTotals[0]);
        Assert.Equal(25.49m, runningTotals[1]);
        Assert.Equal(67.49m, runningTotals[2]);
        Assert.Equal(70.74m, runningTotals[3]);
        Assert.Equal(70.74m, finalTotal);
    }

    [Fact]
    public void ComputeRunningTotals_EmptySequence_YieldsZeroTotalAndNoRunningEntries()
    {
        var (runningTotals, finalTotal) = AggregateTotals.ComputeRunningTotals(Array.Empty<decimal>());

        Assert.Empty(runningTotals);
        Assert.Equal(0m, finalTotal);
    }
}
