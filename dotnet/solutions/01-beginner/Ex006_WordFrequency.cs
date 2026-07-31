namespace FeWoLearning.Exercises.Beginner;

// Exercise 006 — Word Frequency (reference solution).
public static class WordFrequency
{
    public static Dictionary<string, int> Count(string sentence)
    {
        var counts = new Dictionary<string, int>();

        var words = sentence.Split(
            new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            var key = word.ToLowerInvariant();
            counts[key] = counts.TryGetValue(key, out var existing) ? existing + 1 : 1;
        }

        return counts;
    }
}
