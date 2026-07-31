namespace FeWoLearning.Exercises.Advanced;

// Exercise 086 — Micro-benchmark harness (advanced).
// Goal:   Time two algorithm implementations over N iterations using an
//         injected stopwatch abstraction and report which one is faster.
// Drills: dependency injection for testability, delegates, avoiding
//         wall-clock/non-determinism in unit tests, simple statistics.
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
    // Runs algorithmA and algorithmB `iterations` times each, measuring elapsed
    // ticks per run via `stopwatchFactory` (invoked once per algorithm to obtain
    // the IStopwatch instance used across all of that algorithm's iterations),
    // and returns a BenchmarkResult identifying the faster algorithm.
    public static BenchmarkResult Compare(
        string nameA,
        Action algorithmA,
        string nameB,
        Action algorithmB,
        int iterations,
        Func<IStopwatch> stopwatchFactory)
        => throw new NotImplementedException();
}
