using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 005 — LoggerMessageSourceGenerator (logging).
// Goal:   Declare a log event once, as a strongly typed method, and let the source
//         generator write the fast path.
// Drills: [LoggerMessage], EventId and its name, compile-time templates, generated
//         partial methods.
// Passes: calling CacheMiss writes one Warning record whose EventId is 5001 and whose
//                     EventId NAME is "CacheMiss";
//         the record carries the named fields Key and Attempts;
//         the rendered message reads "Cache miss for orders:42 after 3 attempts";
//         and the method CacheMiss itself carries a [LoggerMessage] attribute in the
//                     assembly's metadata.
//
// That last clause is deliberate and it is the only honest way to grade this row.
// Every behavioural fact above can be satisfied by hand-writing
// logger.LogWarning(new EventId(5001, "CacheMiss"), "Cache miss for {Key} ...", ...) -
// an implementation that is not wrong, but is not what the exercise teaches. What the
// generator gives you that the hand-written version does not is the guard, the
// allocation-free argument path, and one declaration site instead of one per call.
// None of that is observable from a log record, so the attribute is read directly.
//
// (The repo has precedent: blazor/ ex069 and ex100 are graded on metadata for the
// same reason - some properties of code are simply not properties of its output.)
//
// To implement: make this class `partial`, declare CacheMiss as a `static partial`
// method with no body, and put [LoggerMessage(...)] on it. The generator writes the
// implementation.
public static partial class Ex005_LoggerMessageSourceGenerator
{
    /// <summary>The event id every cache-miss record must carry.</summary>
    public const int CacheMissEventId = 5001;

    /// <summary>
    /// Write one Warning record reading "Cache miss for {Key} after {Attempts} attempts",
    /// carrying event id <see cref="CacheMissEventId"/> named "CacheMiss".
    /// </summary>
    [LoggerMessage(
        EventId = CacheMissEventId,
        Level = LogLevel.Warning,
        Message = "Cache miss for {Key} after {Attempts} attempts")]
    public static partial void CacheMiss(ILogger logger, string key, int attempts);
}
