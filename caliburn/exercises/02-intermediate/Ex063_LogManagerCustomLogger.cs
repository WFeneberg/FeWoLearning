// Exercise 063 - Log Manager Custom Logger (intermediate).
// Goal:   LogManager's ENTIRE public surface is one settable field: Func<Type, ILog> GetLog.
//         Plugging in your own logging framework means assigning that field once, to a delegate
//         that ignores the requested Type and always hands back your own ILog. ILog's shape is
//         not what you would guess: Info(string format, params object[] args) and Warn(string
//         format, params object[] args) both take a FORMAT STRING plus args - the caller relies
//         on the logger to apply string.Format itself - while Error(Exception exception) takes
//         the EXCEPTION OBJECT, not a message. There is no Debug method on ILog at all.
// Drills: implementing ILog's three methods so they actually apply the formatting contract (not
//         just store the raw format string), assigning LogManager.GetLog so every type reaches
//         the SAME installed logger, and proving a real consumer that calls
//         LogManager.GetLog(typeof(X)) itself - not a private reference kept around from
//         installation - genuinely reaches it.
// Passes: dotnet test --filter FullyQualifiedName~Ex063_
//
// Measured on this machine (Caliburn.Micro 5.0.258): LogManager.GetLog defaults to a delegate
// that hands back the SAME private no-op logger instance for any Type; assigning
// LogManager.GetLog = _ => myLogger makes LogManager.GetLog(typeof(AnyType)).Info("hello {0}",
// "world") reach myLogger, with the formatting already the caller's own string.Format contract
// to fulfil, not something Caliburn does for you.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A logger that records what it was actually asked to log, so a test can inspect it -
/// this IS the exercise: get ILog's real three-method shape right.</summary>
public class Ex063_RecordingLog : ILog
{
    public List<string> InfoMessages { get; } = [];
    public List<string> WarnMessages { get; } = [];
    public List<Exception> Errors { get; } = [];

    /// <summary>Info's real contract: format is a composite format string, args are its
    /// arguments - apply string.Format yourself, the caller does not pre-format it.</summary>
    public void Info(string format, params object[] args) =>
        throw new NotImplementedException("TODO: Ex063 - format and record the info message");

    public void Warn(string format, params object[] args) =>
        throw new NotImplementedException("TODO: Ex063 - format and record the warning message");

    /// <summary>Error takes the EXCEPTION ITSELF, not a format string - there is no Debug
    /// method on ILog at all.</summary>
    public void Error(Exception exception) =>
        throw new NotImplementedException("TODO: Ex063 - record the exception");
}

public class Ex063_LogManagerCustomLogger
{
    /// <summary>Installs log as the ONE logger LogManager hands back for every type, replacing
    /// whatever delegate LogManager.GetLog currently holds entirely.</summary>
    public void Install(ILog log) =>
        throw new NotImplementedException("TODO: Ex063 - LogManager.GetLog = _ => log");
}

/// <summary>A plain consumer that looks its own logger up through LogManager.GetLog, the way
/// real Caliburn-adjacent code does - not a stub, since fetching this way is pre-written
/// plumbing, not the lesson.</summary>
public class Ex063_Worker
{
    public void Process(int count)
    {
        var log = LogManager.GetLog(typeof(Ex063_Worker));
        log.Info("Processed {0} items", count);
        if (count == 0)
            log.Warn("Nothing to process");
    }

    public void Fail(Exception exception) =>
        LogManager.GetLog(typeof(Ex063_Worker)).Error(exception);
}
