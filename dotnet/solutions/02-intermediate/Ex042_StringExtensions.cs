namespace FeWoLearning.Exercises.Intermediate;

// Exercise 042 — String Extensions (reference solution).
public static class StringExtensions
{
    public static bool IsPalindromeExt(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        int left = 0;
        int right = value.Length - 1;

        while (left < right)
        {
            char leftChar = value[left];
            char rightChar = value[right];

            if (!char.IsLetterOrDigit(leftChar))
            {
                left++;
                continue;
            }

            if (!char.IsLetterOrDigit(rightChar))
            {
                right--;
                continue;
            }

            if (char.ToLowerInvariant(leftChar) != char.ToLowerInvariant(rightChar))
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }
}
