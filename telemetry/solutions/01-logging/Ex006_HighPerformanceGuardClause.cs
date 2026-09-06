using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 006 — HighPerformanceGuardClause (logging).
// Goal:   Make the DISABLED path free. A log statement nobody is listening to should
//         cost a level comparison, not a string, a boxed int and a closure.
// Drills: IsEnabled as a guard, LoggerMessage.Define, the cached delegate.
// Passes: with Debug disabled - nothing is written AND describePhase is never called;
//         with Debug enabled  - one Debug record, and describePhase is called exactly
//                     ONCE, not twice;
//         the record carries Phase and Percent as named fields and reads
//                     "Phase copy is 40% complete";
//         and the class caches a delegate built by LoggerMessage.Define in a static
//                     field.
//
// The first clause is the whole exercise. logger.LogDebug("...", describePhase())
// evaluates the argument before the logger ever sees it, so the expensive work happens
// whether or not anyone wanted the record - and it happens on the hot path, in
// production, where Debug is off. The guard has to come first.
//
// The second clause catches the other half: a guard written as
// `if (logger.IsEnabled(...)) Log(logger, describePhase(), ...)` inside a method that
// ALSO evaluates describePhase() to build something else calls it twice.
//
// The fourth clause is graded by reflection, deliberately. What Define buys over a
// hand-written guard - one parsed template instead of one per call, and no boxing of
// the int - leaves no trace whatsoever in a log record. Same stance as ex005, and as
// blazor/ ex069.
public static class Ex006_HighPerformanceGuardClause
{
    /// <summary>The level this event is written at.</summary>
    public const LogLevel Level = LogLevel.Debug;

    /// <summary>
    /// Report progress, but only if anyone is listening.
    ///
    /// If <paramref name="logger"/> has <see cref="Level"/> disabled: do nothing at
    /// all, and in particular do NOT invoke <paramref name="describePhase"/>.
    ///
    /// Otherwise write one record reading "Phase {Phase} is {Percent}% complete",
    /// where Phase is the result of calling <paramref name="describePhase"/> exactly
    /// once. Write it through a delegate built once by
    /// <c>LoggerMessage.Define&lt;string, int&gt;</c> and held in a static field, not
    /// by calling <c>logger.LogDebug</c> here.
    /// </summary>
    // Built once, for the life of the process: the template is parsed here rather
    // than on every call, and the generated delegate takes the int as an int instead
    // of boxing it into an object[].
    private static readonly Action<ILogger, string, int, Exception?> Progress =
        LoggerMessage.Define<string, int>(
            Level,
            new EventId(6001, nameof(ReportProgress)),
            "Phase {Phase} is {Percent}% complete");

    public static void ReportProgress(ILogger logger, Func<string> describePhase, int percent)
    {
        // The guard comes FIRST, before anything expensive is touched. This is the
        // only line that runs on the hot path when Debug is off.
        if (!logger.IsEnabled(Level)) return;

        // Exactly once, into a local. Calling describePhase() inside the argument list
        // of something that also consulted it would pay twice for the same string.
        var phase = describePhase();

        Progress(logger, phase, percent, null);
    }
}
