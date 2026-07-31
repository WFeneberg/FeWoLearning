namespace FeWoLearning.Exercises.Beginner;

// Exercise 017 — Filter Even Squares (reference solution).
public static class FilterEvenSquares
{
    public static List<int> Evaluate(int start, int end) =>
        Enumerable.Range(start, end - start + 1)
            .Where(n => n % 2 == 0)
            .Select(n => n * n)
            .ToList();
}
