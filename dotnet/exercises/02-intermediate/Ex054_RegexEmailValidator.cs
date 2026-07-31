namespace FeWoLearning.Exercises.Intermediate;

// Exercise 054 — Regex Email Validator (intermediate).
// Goal:   Validate whether a string is a plausible email address using
//         Regex.IsMatch. An address must have a non-empty local part
//         (letters, digits, dots, underscores, percent, plus, hyphen),
//         a single '@', a domain of at least two dot-separated labels
//         (letters, digits, hyphens), and a final label of 2-24 letters
//         (the top-level domain). No leading/trailing whitespace, no
//         spaces anywhere, no consecutive dots.
// Drills: regular expressions, Regex.IsMatch, anchoring a pattern.
public static class RegexEmailValidator
{
    public static bool IsValid(string email) => throw new NotImplementedException();
}
