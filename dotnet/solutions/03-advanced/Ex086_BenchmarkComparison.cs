namespace FeWoLearning.Exercises.Advanced;

// Exercise 086 — Micro-benchmark harness (reference solution).
// Each algorithm gets its own IStopwatch instance (from the factory) so
// callers can inject deterministic fakes in tests instead of relying on
// wall-clock timing.
public interface IStopwatch
{
    void Restart();
    long ElapsedTicks { get; }
}

public sealed record BenchmarkResult(
    string NameA,
    double AverageTicksA,
    string NameB,
    double AverageTicksB,
    string FasterName,
    double SpeedupFactor);

public static class BenchmarkComparison
{
    public static BenchmarkResult Compare(
        string nameA,
        Action algorithmA,
        string nameB,
        Action algorithmB,
        int iterations,
        Func<IStopwatch> stopwatchFactory)
    {
        if (algorithmA is null) throw new ArgumentNullException(nameof(algorithmA));
        if (algorithmB is null) throw new ArgumentNullException(nameof(algorithmB));
        if (stopwatchFactory is null) throw new ArgumentNullException(nameof(stopwatchFactory));
        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "Must be positive.");

        double averageA = RunAndAverage(algorithmA, iterations, stopwatchFactory);
        double averageB = RunAndAverage(algorithmB, iterations, stopwatchFactory);

        bool aIsFaster = averageA <= averageB;
        double fasterAvg = aIsFaster ? averageA : averageB;
        double slowerAvg = aIsFaster ? averageB : averageA;
        string fasterName = aIsFaster ? nameA : nameB;

        double speedup = fasterAvg == 0
            ? (slowerAvg == 0 ? 1.0 : double.PositiveInfinity)
            : slowerAvg / fasterAvg;

        return new BenchmarkResult(nameA, averageA, nameB, averageB, fasterName, speedup);
    }

    private static double RunAndAverage(Action algorithm, int iterations, Func<IStopwatch> stopwatchFactory)
    {
        var stopwatch = stopwatchFactory();
        long totalTicks = 0;
        for (int i = 0; i < iterations; i++)
        {
            stopwatch.Restart();
            algorithm();
            totalTicks += stopwatch.ElapsedTicks;
        }
        return (double)totalTicks / iterations;
    }
}
