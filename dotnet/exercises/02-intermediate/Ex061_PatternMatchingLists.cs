namespace FeWoLearning.Exercises.Intermediate;

// Exercise 061 — Pattern Matching over Lists (intermediate).
// Goal:   Classify an integer sequence into a descriptive label using C#
//         list patterns ([], [x], [first, .., last]) combined with property
//         patterns (e.g. { Length: ... }) and `when` guards, evaluated in
//         this priority order:
//           1. []                                  -> "Empty"
//           2. [x]                                 -> "Single:{x}"
//           3. [a, b] where a == b                 -> "Pair:Equal"
//           4. [first, .., last] where first==last
//              (length >= 3)                       -> "Bookended"
//           5. length > 4 and strictly non-decreasing
//              (ascending or flat)                 -> "Sorted"
//           6. anything else                       -> "Other"
// Drills: list patterns, slice patterns, property patterns, pattern guards.
public static class PatternMatchingLists
{
    public static string Classify(int[] sequence) => throw new NotImplementedException();
}
