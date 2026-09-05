using System.Text;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 051 — SecretRedactionInLogs (reference solution).
public static class Ex051_SecretRedactionInLogs
{
    private const string RedactedMarker = "***REDACTED***";

    // Exact key names only, matched case-insensitively - a Contains-style check
    // would also catch "passwordPolicyVersion", which is a different key that
    // happens to share a prefix, not a sensitive one.
    private static readonly string[] SensitiveKeyNames = ["password", "apikey", "authorization", "token"];

    public static string Redact(string message, IReadOnlyDictionary<string, object?> state)
    {
        // A caller who string-interpolated a secret straight into the message
        // (instead of routing it through `state`) still leaks it unless the raw
        // message text itself is scrubbed for every sensitive value `state`
        // names.
        var scrubbedMessage = message;
        foreach (var pair in state)
        {
            if (!IsSensitiveKey(pair.Key)) continue;

            var text = pair.Value?.ToString();
            if (string.IsNullOrEmpty(text)) continue;

            scrubbedMessage = scrubbedMessage.Replace(text, RedactedMarker, StringComparison.Ordinal);
        }

        var builder = new StringBuilder(scrubbedMessage);

        foreach (var pair in state.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            builder.Append(" | ").Append(pair.Key).Append('=');

            if (IsSensitiveKey(pair.Key))
            {
                builder.Append(RedactedMarker);
                continue;
            }

            var rendered = pair.Value?.ToString() ?? "null";

            // Neutralise CR/LF in every rendered value - not just sensitive ones -
            // so a value cannot forge what looks like a second, fabricated log
            // line in whatever sink ultimately writes this string one line at a
            // time.
            builder.Append(Neutralize(rendered));
        }

        return builder.ToString();
    }

    private static bool IsSensitiveKey(string key) =>
        SensitiveKeyNames.Contains(key, StringComparer.OrdinalIgnoreCase);

    private static string Neutralize(string value) =>
        value.Replace("\r", "\\r", StringComparison.Ordinal).Replace("\n", "\\n", StringComparison.Ordinal);
}
