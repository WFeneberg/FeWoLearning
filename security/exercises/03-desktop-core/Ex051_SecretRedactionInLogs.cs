namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 051 — SecretRedactionInLogs (desktop-core).
// Goal:   Render a structured log message plus its state dictionary into one
//         string, without ever emitting a sensitive value - not through its own
//         key, and not because someone accidentally baked it into the message
//         text itself. Also make sure a value can't forge a fake extra log line
//         by embedding a CR or LF.
// Drills: structured logging, redaction of sensitive values, log injection.
// Passes: attack facts   - values under keys named password, apiKey,
//                          authorization or token (matched case-insensitively,
//                          by exact key name) never appear in the output; a
//                          sensitive value that also appears verbatim inside the
//                          message text itself (not just via `state`) is
//                          redacted there too; a CR or LF inside any value is
//                          neutralised so it cannot be used to forge an
//                          extra, fake log line;
//         use facts      - non-sensitive keys and their values do appear in the
//                          output; the message's own non-sensitive text is
//                          preserved verbatim; and a key literally named
//                          `passwordPolicyVersion` is *not* redacted - it is a
//                          different key, not a "password"-shaped one, which is
//                          exactly the case a naive `Contains("password")` check
//                          gets wrong.
public static class Ex051_SecretRedactionInLogs
{
    public static string Redact(string message, IReadOnlyDictionary<string, object?> state) =>
        throw new NotImplementedException(
            "TODO: Ex051 - redact any state value whose key exactly matches (case-insensitively) password/apiKey/" +
            "authorization/token, scrub the same values out of `message` itself, neutralise CR/LF in every value, " +
            "and otherwise append the remaining key=value pairs to the message");
}
