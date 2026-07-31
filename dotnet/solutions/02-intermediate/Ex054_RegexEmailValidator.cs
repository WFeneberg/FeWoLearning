using System.Text.RegularExpressions;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 054 — Regex Email Validator (reference solution).
public static class RegexEmailValidator
{
    private static readonly Regex Pattern = new(
        @"^[A-Za-z0-9_%+-]+(?:\.[A-Za-z0-9_%+-]+)*@(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,24}$",
        RegexOptions.Compiled);

    public static bool IsValid(string email) => !string.IsNullOrEmpty(email) && Pattern.IsMatch(email);
}
