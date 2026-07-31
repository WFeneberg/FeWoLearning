namespace FeWoLearning.Exercises.Intermediate;

// Exercise 043 — SpanSliceParser (reference solution).
public static class SpanSliceParser
{
    public static int[] ParseInts(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.Length == 0)
        {
            return Array.Empty<int>();
        }

        ReadOnlySpan<char> span = input.AsSpan();

        // First pass: count entries without allocating anything.
        var count = 1;
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == ',')
            {
                count++;
            }
        }

        var result = new int[count];
        var index = 0;
        var remaining = span;

        while (true)
        {
            var commaPos = remaining.IndexOf(',');
            ReadOnlySpan<char> token = commaPos == -1 ? remaining : remaining[..commaPos];

            result[index++] = int.Parse(token.Trim(), System.Globalization.NumberStyles.Integer);

            if (commaPos == -1)
            {
                break;
            }

            remaining = remaining[(commaPos + 1)..];
        }

        return result;
    }
}
