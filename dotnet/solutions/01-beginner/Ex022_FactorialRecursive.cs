namespace FeWoLearning.Exercises.Beginner;

// Exercise 022 — FactorialRecursive (reference solution).
public static class FactorialRecursive
{
    public static long Compute(int n)
    {
        if (n < 0)
        {
            throw new ArgumentException("n must be non-negative.", nameof(n));
        }

        return n <= 1 ? 1L : n * Compute(n - 1);
    }
}
