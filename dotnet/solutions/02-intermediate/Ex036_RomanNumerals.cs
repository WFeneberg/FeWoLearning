using System.Text;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 036 — Roman numerals (reference solution).
public static class RomanNumerals
{
    private static readonly (int Value, string Symbol)[] Table =
    {
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
        (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I"),
    };

    public static string ToRoman(int value)
    {
        if (value is < 1 or > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Must be 1..3999.");

        var sb = new StringBuilder();
        foreach (var (v, symbol) in Table)
        {
            while (value >= v)
            {
                sb.Append(symbol);
                value -= v;
            }
        }
        return sb.ToString();
    }
}
