using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex086_BenchmarkComparisonTests
{
    // Deterministic fake: always reports the same fixed elapsed-ticks value,
    // regardless of how long the measured action actually takes. This removes
    // wall-clock timing from the test entirely.
    private sealed class FakeStopwatch : IStopwatch
    {
        private readonly long _fixedTicks;
        public FakeStopwatch(long fixedTicks) => _fixedTicks = fixedTicks;
        public void Restart() { /* no-op: reading is fixed */ }
        public long ElapsedTicks => _fixedTicks;
    }

    private static Func<IStopwatch> FactoryFor(long ticksA, long ticksB)
    {
        var callCount = 0;
        return () =>
        {
            callCount++;
            return callCount == 1 ? new FakeStopwatch(ticksA) : new FakeStopwatch(ticksB);
        };
    }

    [Fact]
    public void Compare_IdentifiesFasterAlgorithm_WhenSecondIsFaster()
    {
        var result = BenchmarkComparison.Compare(
            "SlowSort", () => { },
            "FastSort", () => { },
            iterations: 5,
            stopwatchFactory: FactoryFor(ticksA: 10, ticksB: 3));

        Assert.Equal("SlowSort", result.NameA);
        Assert.Equal(10.0, result.AverageTicksA);
        Assert.Equal("FastSort", result.NameB);
        Assert.Equal(3.0, result.AverageTicksB);
        Assert.Equal("FastSort", result.FasterName);
        Assert.Equal(10.0 / 3.0, result.SpeedupFactor, 3);
    }

    [Fact]
    public void Compare_IdentifiesFasterAlgorithm_WhenFirstIsFaster()
    {
        var result = BenchmarkComparison.Compare(
            "QuickAlgo", () => { },
            "SlowAlgo", () => { },
            iterations: 4,
            stopwatchFactory: FactoryFor(ticksA: 2, ticksB: 8));

        Assert.Equal("QuickAlgo", result.FasterName);
        Assert.Equal(2.0, result.AverageTicksA);
        Assert.Equal(8.0, result.AverageTicksB);
        Assert.Equal(4.0, result.SpeedupFactor, 3);
    }

    [Fact]
    public void Compare_AveragesTicksAcrossIterations()
    {
        // 3 iterations at 6 ticks each = total 18, average 6.
        var result = BenchmarkComparison.Compare(
            "A", () => { },
            "B", () => { },
            iterations: 3,
            stopwatchFactory: FactoryFor(ticksA: 6, ticksB: 6));

        Assert.Equal(6.0, result.AverageTicksA);
        Assert.Equal(6.0, result.AverageTicksB);
        Assert.Equal(1.0, result.SpeedupFactor, 3);
    }

    [Fact]
    public void Compare_RejectsNonPositiveIterations()
        => Assert.Throws<ArgumentOutOfRangeException>(() =>
            BenchmarkComparison.Compare(
                "A", () => { },
                "B", () => { },
                iterations: 0,
                stopwatchFactory: FactoryFor(1, 1)));

    [Fact]
    public void Compare_RejectsNullStopwatchFactory()
        => Assert.Throws<ArgumentNullException>(() =>
            BenchmarkComparison.Compare(
                "A", () => { },
                "B", () => { },
                iterations: 5,
                stopwatchFactory: null!));
}
