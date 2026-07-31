namespace FeWoLearning.Exercises.Intermediate;

// Exercise 065 — Memoized Fibonacci (reference solution).
public static class MemoizedFibonacci
{
    private static readonly Dictionary<int, long> _cache = new();

    public static int CallCount { get; private set; }

    public static long Calculate(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative.");
        }

        // Each top-level call starts a fresh measurement, but the memo
        // cache itself persists across calls (as a real memoized
        // Fibonacci implementation would), so later calls for larger n
        // reuse work already done for smaller n.
        CallCount = 0;
        return Compute(n);
    }

    private static long Compute(int n)
    {
        CallCount++;

        if (n <= 1)
        {
            return n;
        }

        if (_cache.TryGetValue(n, out var cached))
        {
            return cached;
        }

        var result = Compute(n - 1) + Compute(n - 2);
        _cache[n] = result;
        return result;
    }
}
