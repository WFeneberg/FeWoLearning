namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 022 — OpenRedirectGuard (reference solution).
public static class Ex022_OpenRedirectGuard
{
    public static string SafeReturnUrl(string? candidate, string fallback)
    {
        if (string.IsNullOrEmpty(candidate))
            return fallback;

        // Some browsers (historically Edge/IE, and plenty of naive URL parsers
        // since) treat a backslash exactly like a forward slash. Normalise only
        // for the shape check below - "/\evil.example" and "http:/\/\evil.example"
        // are protocol-relative in disguise even though neither contains a
        // literal "//". The accepted return value is always the untouched
        // original candidate, never this normalised one.
        var normalised = candidate.Replace('\\', '/');

        // Anything not rooted at exactly one leading slash - an absolute URL, a
        // scheme like "javascript:", or a protocol-relative "//host/..." - is
        // rejected here before it is ever handed to Uri for a second opinion.
        if (!normalised.StartsWith('/') || normalised.StartsWith("//"))
            return fallback;

        // Belt and braces: if the normalised form still parses as an absolute
        // URI (a scheme this check did not anticipate), reject it too.
        if (Uri.TryCreate(normalised, UriKind.Absolute, out _))
            return fallback;

        return candidate;
    }
}
