namespace FeWoLearning.Exercises.Intermediate;

// Exercise 064 — Custom Comparer Sort (intermediate).
// Goal:   Implement an IComparer<string> that orders strings primarily by
//         length (shortest first) and, for strings of equal length, falls
//         back to ordinary (ordinal) alphabetical order.
// Drills: IComparer<T>, List<T>.Sort(IComparer<T>), tie-break comparisons.
public sealed class LengthThenAlphaComparer : IComparer<string>
{
    public int Compare(string? x, string? y) => throw new NotImplementedException();
}

public static class CustomComparerSort
{
    public static List<string> SortByLengthThenAlpha(IEnumerable<string> values)
        => throw new NotImplementedException();
}
