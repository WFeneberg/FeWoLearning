namespace FeWoLearning.Exercises.Beginner;

// Exercise 016 — Fibonacci Sequence (reference solution).
public static class FibonacciSequence
{
    public static IEnumerable<long> Generate(int count)
    {
        long previous = 0;
        long current = 1;

        for (var i = 0; i < count; i++)
        {
            yield return previous;
            (previous, current) = (current, previous + current);
        }
    }
}
