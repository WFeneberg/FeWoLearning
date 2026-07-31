namespace FeWoLearning.Exercises.Advanced;

// Exercise 080 — Stackalloc integer parser (reference solution).
// Walks the input span looking for ',' separators, trims each slice, and
// parses it directly with int.Parse(ReadOnlySpan<char>) — no substrings,
// no arrays, no LINQ. The result span is written in place into the
// caller-provided (typically stackalloc'd) destination.
public static class StackallocParser
{
    public const int MaxValues = 8;

    public static int Parse(ReadOnlySpan<char> input, Span<int> destination)
    {
        int written = 0;
        ReadOnlySpan<char> remaining = input;

        while (true)
        {
            int commaIndex = remaining.IndexOf(',');
            ReadOnlySpan<char> entry = commaIndex >= 0 ? remaining[..commaIndex] : remaining;
            entry = entry.Trim();

            if (written >= destination.Length)
                throw new ArgumentException("Destination span is too small for the number of values.", nameof(destination));

            destination[written++] = int.Parse(entry);

            if (commaIndex < 0)
                break;

            remaining = remaining[(commaIndex + 1)..];
        }

        return written;
    }
}
