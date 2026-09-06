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
        ILogger logger, string userId, string email, string cardNumber, bool succeeded) =>
        throw new NotImplementedException(
            "TODO: Ex009 - redact the sensitive FIELDS before logging, never the finished text");
}
