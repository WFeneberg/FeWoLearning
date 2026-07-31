namespace FeWoLearning.Exercises.Intermediate;

// Exercise 043 — SpanSliceParser (intermediate).
// Goal:   Parse a comma-separated list of integers (e.g. "10,-3,42,0,7") into an
//         int[] by slicing a ReadOnlySpan<char> over the input string, without
//         allocating any intermediate strings (no Split, no Substring).
// Drills: Span<T>/ReadOnlySpan<T> slicing, IndexOf on spans, int.Parse(ReadOnlySpan<char>),
//         manual tokenizing loops, avoiding allocations on hot parsing paths.
public static class SpanSliceParser
{
    public static int[] ParseInts(string input) => throw new NotImplementedException();
}
