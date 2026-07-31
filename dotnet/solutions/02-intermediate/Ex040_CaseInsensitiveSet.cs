using System;
using System.Collections.Generic;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 040 — Case-Insensitive Set (reference solution).
public sealed class CaseInsensitiveComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x is null || y is null)
        {
            return x is null && y is null;
        }

        return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
        => obj.ToUpperInvariant().GetHashCode();
}

public static class CaseInsensitiveSet
{
    public static HashSet<string> Build(IEnumerable<string> values)
    {
        var set = new HashSet<string>(new CaseInsensitiveComparer());
        foreach (var value in values)
        {
            set.Add(value);
        }

        return set;
    }
}
