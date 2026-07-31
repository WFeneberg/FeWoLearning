namespace FeWoLearning.Exercises.Advanced;

// Exercise 080 — Stackalloc integer parser (advanced).
// Goal:   Parse a comma-separated sequence of integers directly into a
//         caller-supplied Span<int> (typically backed by `stackalloc`),
//         without allocating any intermediate arrays, lists, or substrings.
// Drills: ref structs, Span<T>/ReadOnlySpan<T>, stackalloc, slicing,
//         MemoryExtensions parsing (no LINQ, no string.Split allocations).
public static class StackallocParser
{
    // Upper bound a caller can safely stackalloc for this parser.
    public const int MaxValues = 8;

    // Parses up to `destination.Length` comma-separated integers from `input`
    // into `destination`, returning the number of values written.
    // Whitespace around each entry must be ignored.
    // Throws ArgumentException if `input` contains more values than `destination` can hold.
    // Throws FormatException if any entry is not a valid integer.
    public static int Parse(ReadOnlySpan<char> input, Span<int> destination) => throw new NotImplementedException();
}
