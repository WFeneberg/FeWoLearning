using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 009 — RedactionAndPii (logging).
// Goal:   Keep personal data out of the log by deciding on the FIELD, never on the
//         value.
// Drills: structural redaction, why pattern-matching the rendered text is the wrong
//         layer.
// Passes: UserId and Outcome carry their real values;
//         Email and CardNumber carry exactly "[redacted]";
//         the rendered message contains neither the real address nor the real number;
//         a sensitive field whose value looks harmless is STILL redacted;
//         and a safe field whose value happens to look like an address is NOT.
//
// The last two clauses are the exercise, and they are a matched pair on purpose.
//
// The tempting implementation is a regex over the finished message that hunts for
// things shaped like an email or a card. It fails in both directions and both
// failures are silent. It misses "n/a" sitting in the Email field - which is a leak,
// because the field is sensitive whatever happens to be in it today - and it mangles a
// user id that legitimately contains an "@", which destroys real data and, worse,
// teaches everyone to stop trusting the log.
//
// The field name is a decision made once, by whoever declared the field. The value is
// a guess made on every record. Redact on the decision.
public static class Ex009_RedactionAndPii
{
    /// <summary>What every redacted value is replaced with.</summary>
    public const string Placeholder = "[redacted]";

    /// <summary>
    /// Field names whose values must never reach a sink, whatever they contain.
    /// </summary>
    public static readonly string[] SensitiveFields = ["Email", "CardNumber"];

    /// <summary>
    /// Write ONE Information record about a sign-in attempt, reading
    /// "Sign-in for {UserId} ({Email}, card {CardNumber}): {Outcome}".
    ///
    /// UserId keeps its value. Outcome is "succeeded" or "failed", from
    /// <paramref name="succeeded"/>. Email and CardNumber are replaced by
    /// <see cref="Placeholder"/> - both as the field value and, therefore, in the
    /// rendered message.
    /// </summary>
    public static void LogSignIn(
        ILogger logger, string userId, string email, string cardNumber, bool succeeded)
    {
        // Every argument goes through the same gate, and the gate looks only at the
        // field name. Nothing downstream ever sees the real address or number, so
        // there is no later stage that could leak them - not the field, not the
        // rendered message, not a sink that decides to store both.
        logger.LogInformation(
            "Sign-in for {UserId} ({Email}, card {CardNumber}): {Outcome}",
            Redact("UserId", userId),
            Redact("Email", email),
            Redact("CardNumber", cardNumber),
            Redact("Outcome", succeeded ? "succeeded" : "failed"));
    }

    /// <summary>
    /// Redact by NAME. The value is never inspected - that is the entire point, and
    /// the reason this cannot be fooled by a harmless-looking secret, nor fooled into
    /// destroying a legitimate value that happens to look like one.
    /// </summary>
    private static string Redact(string field, string value) =>
        SensitiveFields.Contains(field, StringComparer.Ordinal) ? Placeholder : value;
}
