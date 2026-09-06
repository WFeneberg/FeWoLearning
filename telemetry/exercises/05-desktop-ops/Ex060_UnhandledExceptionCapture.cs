using Microsoft.Extensions.Logging;
using System.Windows.Threading;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 060 — UnhandledExceptionCapture (desktop-ops).
// Goal:   Get exactly one crash report out of a failure that several hooks can see, and
//         none out of a failure nobody saw.
// Drills: Dispatcher.UnhandledException, marking it handled, deduplicating by instance.
// Passes: an exception thrown inside a dispatcher callback is captured once, with its
//                     source and the exception itself;
//         the dispatcher keeps working afterwards, because the handler marked it handled;
//         the SAME exception instance offered to a second hook records nothing more;
//         and a DIFFERENT exception is recorded normally.
//
// The third and fourth clauses are the row, and the title's "one record, not three or
// none". A desktop application has several places an escaping exception can surface -
// the dispatcher, a faulted task nobody awaited, the AppDomain on the way down - and one
// failure often reaches more than one of them. Three reports of one crash is not three
// times the information; it is a support engineer counting an incident three times and a
// deduplication rule somebody has to invent later, in a query, from worse data.
//
// Deduplicating by INSTANCE is what makes it safe. A rule like "report only the first
// exception" loses every subsequent failure; a rule like "report only one per message"
// merges genuinely separate incidents that happen to say the same thing. The identity of
// the object is the only thing that means "this is the same failure arriving twice".
//
// The second clause is the one that decides whether the application survives. An
// unhandled dispatcher exception tears the process down; marking it handled is what turns
// a crash into a recorded fault - and is also a decision to keep running with state you
// no longer trust, which is why it belongs at the top level and nowhere else.
//
// Note what this row does NOT cover, and why. AppDomain.UnhandledException cannot be
// exercised without ending the test process, and TaskScheduler.UnobservedTaskException
// fires from a finalizer, which caliburn/ measured to behave differently under Server GC
// and with tiered compilation disabled. Both funnel into the same Capture, which is the
// part worth grading.
public static class Ex060_UnhandledExceptionCapture
{
    /// <summary>The category crash reports are written under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex060";

    /// <summary>The field naming which hook saw the failure.</summary>
    public const string SourceField = "FaultSource";

    /// <summary>The dispatcher hook.</summary>
    public const string DispatcherSource = "dispatcher";

    /// <summary>The faulted-task hook.</summary>
    public const string TaskSource = "task";

    /// <summary>The last-resort hook.</summary>
    public const string DomainSource = "appdomain";

    /// <summary>The constant template every crash report uses.</summary>
    public const string Template = "Unhandled failure surfaced by {FaultSource}";

    /// <summary>
    /// Write ONE Critical record for <paramref name="failure"/>, using
    /// <see cref="Template"/> with <paramref name="source"/> as
    /// <see cref="SourceField"/> and the exception passed as the exception argument.
    ///
    /// The same exception INSTANCE offered again - because it reached a second hook -
    /// records nothing. A different instance records normally, even if it says the same
    /// thing.
    /// </summary>
    public static void Capture(ILogger logger, Exception failure, string source) =>
        throw new NotImplementedException(
            "TODO: Ex060 - report the failure once per exception instance, whatever hook it came from");

    /// <summary>
    /// Attach to <paramref name="dispatcher"/> so that an exception escaping one of its
    /// callbacks is captured with source <see cref="DispatcherSource"/> and marked
    /// handled, leaving the dispatcher usable.
    /// </summary>
    public static void AttachTo(Dispatcher dispatcher, ILogger logger) =>
        throw new NotImplementedException(
            "TODO: Ex060 - capture the dispatcher's unhandled exceptions and let it carry on");
}
