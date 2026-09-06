using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 018 — StatusAndException (diagnostics).
// Goal:   Record that something failed without changing whether it failed.
// Drills: SetStatus, the "exception" event and its conventional tags, not swallowing.
// Passes: work that succeeds leaves the activity with status Ok and no exception event;
//         work that throws leaves the activity with status Error and the exception's
//                     message as the status description;
//         the failure carries an event named "exception" tagged with the conventional
//                     exception.type and exception.message keys;
//         and the exception is RETHROWN - Execute never swallows it.
//
// The last clause is the bug this row exists for, and it is a bad one. The natural
// shape for "record the failure" is try/catch, and a catch block that records and then
// falls off the end has turned an exception into a silent success. The caller carries
// on with a null it did not expect; the trace, meanwhile, looks perfect - a red span
// sitting under a green one. Observability code must be transparent to control flow.
//
// The third clause is about names, not behaviour. Every backend, dashboard and alert
// rule keys on the conventional exception.type / exception.message attributes inside an
// event literally called "exception". Invent your own and the data is technically all
// there and practically invisible: nothing will find it, and nothing will tell you so.
// Activity.AddException does this for you.
public static class Ex018_StatusAndException
{
    /// <summary>The name this exercise's source is registered under.</summary>
    public const string SourceName = "fewolearning.telemetry.ex018";

    /// <summary>The name of the activity wrapping the work.</summary>
    public const string ActivityName = "unit-of-work";

    /// <summary>The conventional name of an exception event.</summary>
    public const string ExceptionEventName = "exception";

    /// <summary>The conventional tag carrying the exception's type name.</summary>
    public const string ExceptionTypeTag = "exception.type";

    /// <summary>The conventional tag carrying the exception's message.</summary>
    public const string ExceptionMessageTag = "exception.message";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Run <paramref name="work"/> inside an <see cref="ActivityName"/> activity and
    /// return that activity.
    ///
    /// If it returns normally, set the status to
    /// <see cref="ActivityStatusCode.Ok"/>. If it throws, record the exception on the
    /// activity, set the status to <see cref="ActivityStatusCode.Error"/> with the
    /// exception's message as the description - and let the exception continue on its
    /// way.
    /// </summary>
    public static Activity? Execute(Action work)
    {
        // `using`, so the activity is stopped on BOTH paths. A using that only covers
        // the happy path leaves the failing span running for the life of the process.
        using var activity = Source.StartActivity(ActivityName);

        try
        {
            work();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return activity;
        }
        catch (Exception error)
        {
            // AddException writes the conventional "exception" event with
            // exception.type and exception.message, which is what every backend keys
            // on. A hand-rolled event with invented names is technically all there and
            // practically invisible.
            activity?.AddException(error);
            activity?.SetStatus(ActivityStatusCode.Error, error.Message);

            // Bare `throw`, not `throw error`: the original stack trace survives. And
            // it must throw at all - a catch that records and falls off the end has
            // turned a failure into a silent success.
            throw;
        }
    }
}
