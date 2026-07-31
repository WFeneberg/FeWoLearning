namespace FeWoLearning.Exercises.Beginner;

// Exercise 019 — ParseOrDefault (reference solution).
public static class ParseOrDefault
{
    public static int ParseIntOrDefault(string? text, int fallback)
        => int.TryParse(text, out var value) ? value : fallback;
}
