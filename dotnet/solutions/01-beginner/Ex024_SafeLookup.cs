namespace FeWoLearning.Exercises.Beginner;

// Exercise 024 — SafeLookup (reference solution).
public static class SafeLookup
{
    public static string? TryFind(string[] items, Func<string, bool> predicate)
    {
        foreach (var item in items)
        {
            if (predicate(item))
            {
                return item;
            }
        }

        return null;
    }
}
