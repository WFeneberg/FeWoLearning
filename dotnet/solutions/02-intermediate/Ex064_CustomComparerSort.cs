namespace FeWoLearning.Exercises.Intermediate;

// Exercise 064 — Custom Comparer Sort (reference solution).
public sealed class LengthThenAlphaComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var lengthComparison = x.Length.CompareTo(y.Length);
        return lengthComparison != 0
            ? lengthComparison
            : string.CompareOrdinal(x, y);
    }
}

public static class CustomComparerSort
{
    public static List<string> SortByLengthThenAlpha(IEnumerable<string> values)
    {
        var list = new List<string>(values);
        list.Sort(new LengthThenAlphaComparer());
        return list;
    }
}
