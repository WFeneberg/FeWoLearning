// Exercise 067 - Global exception hooks on the dispatcher. REFERENCE SOLUTION.
// Goal:   Catch an exception that escapes a dispatcher-queued callback before it tears down the
//         whole app - and, separately, let a genuinely fatal kind of exception escape anyway,
//         rather than swallow everything indiscriminately. There is no Application in this
//         harness (see README.md, "What the harness cannot do"), so Application.
//         DispatcherUnhandledException cannot be exercised here - but Dispatcher.UnhandledException
//         and Dispatcher.UnhandledExceptionFilter are members of Dispatcher itself, not
//         Application, and work exactly the same with no Application anywhere in sight.
// Drills: Dispatcher.UnhandledExceptionFilter (deciding whether an exception is even OFFERED to
//         the handler at all, via DispatcherUnhandledExceptionFilterEventArgs.RequestCatch) and
//         Dispatcher.UnhandledException (logging and marking handled everything the filter did not
//         already decline).

using System.Windows.Threading;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ready to use - a marker for the one kind of exception this row's hooks deliberately let
/// escape rather than log-and-continue past, the way a real app might reserve for a corrupted-
/// state condition it does not trust itself to keep running after.
/// </summary>
public sealed class Ex067_FatalException(string message) : Exception(message);

public static class Ex067_GlobalExceptionHooks
{
    /// <summary>
    /// Installs both dispatcher-level exception hooks on <paramref name="dispatcher"/>:
    /// <list type="bullet">
    /// <item>an <see cref="Dispatcher.UnhandledExceptionFilter"/> handler that sets
    /// <c>e.RequestCatch = false</c> for an <see cref="Ex067_FatalException"/> - which skips the
    /// handler below entirely and lets it propagate - and leaves <c>RequestCatch</c> untouched
    /// (its default is <see langword="true"/>) for anything else;</item>
    /// <item>an <see cref="Dispatcher.UnhandledException"/> handler that, for every exception the
    /// filter did not already decline, appends it to <paramref name="log"/> and sets
    /// <c>e.Handled = true</c> so dispatcher processing continues instead of tearing down.</item>
    /// </list>
    /// Returns an <see cref="IDisposable"/> whose <see cref="IDisposable.Dispose"/> unsubscribes
    /// both handlers from <paramref name="dispatcher"/>.
    /// </summary>
    public static IDisposable Install(Dispatcher dispatcher, IList<Exception> log)
    {
        DispatcherUnhandledExceptionFilterEventHandler filter = (_, e) =>
        {
            if (e.Exception is Ex067_FatalException)
            {
                e.RequestCatch = false;
            }
        };
        DispatcherUnhandledExceptionEventHandler handler = (_, e) =>
        {
            log.Add(e.Exception);
            e.Handled = true;
        };

        dispatcher.UnhandledExceptionFilter += filter;
        dispatcher.UnhandledException += handler;

        return new Unsubscriber(dispatcher, filter, handler);
    }

    private sealed class Unsubscriber(
        Dispatcher dispatcher,
        DispatcherUnhandledExceptionFilterEventHandler filter,
        DispatcherUnhandledExceptionEventHandler handler) : IDisposable
    {
        public void Dispose()
        {
            dispatcher.UnhandledExceptionFilter -= filter;
            dispatcher.UnhandledException -= handler;
        }
    }
}
