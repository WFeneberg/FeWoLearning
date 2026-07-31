namespace FeWoLearning.Exercises.Intermediate;

// Exercise 061 — Pattern Matching over Lists (reference solution).
public static class PatternMatchingLists
{
    public static string Classify(int[] sequence) => sequence switch
    {
        [] => "Empty",
        [var x] => $"Single:{x}",
        [var a, var b] when a == b => "Pair:Equal",
        [var first, .., var last] when first == last => "Bookended",
        { Length: > 4 } when IsNonDecreasing(sequence) => "Sorted",
        _ => "Other",
    };

    private static bool IsNonDecreasing(int[] sequence)
    {
        for (var i = 1; i < sequence.Length; i++)
        {
            if (sequence[i] < sequence[i - 1])
            {
                return false;
            }
        }

        return true;
    }
}
