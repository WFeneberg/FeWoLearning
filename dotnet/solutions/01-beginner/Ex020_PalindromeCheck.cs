namespace FeWoLearning.Exercises.Beginner;

// Exercise 020 — PalindromeCheck (reference solution).
public static class PalindromeCheck
{
    public static bool IsPalindrome(string input)
    {
        var chars = input
            .Where(c => !char.IsWhiteSpace(c))
            .Select(char.ToLowerInvariant)
            .ToArray();

        var reversed = chars.Reverse();
        return chars.SequenceEqual(reversed);
    }
}
