using System.Collections.Generic;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 040 — Case-Insensitive Set (intermediate).
// Goal:   Implement a custom IEqualityComparer<string> that treats strings as
//         equal ignoring case, then use it to build a HashSet<string> that
//         de-duplicates entries differing only by casing.
// Drills: IEqualityComparer<T>, HashSet<T> with a custom comparer, hash-code
//         contracts (Equals/GetHashCode must agree).
public sealed class CaseInsensitiveComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y) => throw new NotImplementedException();

    public int GetHashCode(string obj) => throw new NotImplementedException();
}

public static class CaseInsensitiveSet
{
    // Builds a HashSet<string> using CaseInsensitiveComparer so that entries
    // differing only by case are treated as duplicates.
    public static HashSet<string> Build(IEnumerable<string> values) => throw new NotImplementedException();
}
