using System.Diagnostics;
using System.Windows.Threading;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 061 — CommandActivityTracing (desktop-ops).
// Goal:   Trace one user action end to end, across the thread hops a desktop application
//         makes without anybody writing them down.
// Drills: one span per command, ambient context across the dispatcher and across await.
// Passes: a command produces ONE span covering all of it;
//         work running off the UI thread sees that span as Activity.Current, with nothing
//                     passing it along;
//         a span started by that work is its CHILD rather than a root;
//         after the hop back to the UI thread it is still current;
//         and once the command is over, Activity.Current is null again.
//
// The second and fourth clauses are measured rather than assumed, and they are better
// news than most people expect: Activity.Current is AsyncLocal and rides on the
// ExecutionContext, which both Dispatcher.InvokeAsync and Dispatcher.BeginInvoke capture.
// So a command that starts on the UI thread, does its work on a thread-pool thread and
// comes back to update the view stays one trace, with no correlation id threaded through
// any signature.
//
// Where it does NOT flow is worth knowing too, because the failure is silent: a delegate
// captured BEFORE the span started carries the context from then, a raw
// `new Thread(...)` does not flow it at all, and anything queued to a timer that was
// created earlier belongs to whenever that happened. The rule is that the context travels
// with the CAPTURE, not with the call.
//
// The last clause is the discipline from row 016 in the place it matters most. A desktop
// application's UI thread lives for hours and runs thousands of commands; one that leaks
// its span makes every later command a child of it, and the trace grows until the backend
// refuses it.
public static class Ex061_CommandActivityTracing
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex061";

    /// <summary>The span covering the whole command.</summary>
    public const string CommandSpanName = "command.save";

    /// <summary>The span the off-thread work opens inside it.</summary>
    public const string WorkSpanName = "save.persist";

    /// <summary>The attribute recording which thread the command finished on.</summary>
    public const string CompletedOnUiThreadTag = "command.completed_on_ui_thread";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Run one command inside a single <see cref="CommandSpanName"/> span:
    ///
    ///   - hand off to <paramref name="save"/> on a thread-pool thread and await it;
    ///   - come back to <paramref name="dispatcher"/> to finish;
    ///   - tag the span <see cref="CompletedOnUiThreadTag"/> with whether the finish
    ///     really happened on that dispatcher's thread.
    ///
    /// Nothing passes the span along by hand - the point is that it travels on its own.
    /// Leave <see cref="Activity.Current"/> as it was found.
    /// </summary>
    public static async Task<string> ExecuteSaveAsync(Dispatcher dispatcher, Func<Task<string>> save)
    {
        using var command = Source.StartActivity(CommandSpanName);

        // Nothing is passed along. Activity.Current is AsyncLocal and rides on the
        // ExecutionContext, which both the await and the dispatcher hop capture.
        var result = await save().ConfigureAwait(false);

        // Back to the UI thread, the way a real command has to be to touch the view.
        await dispatcher.InvokeAsync(() =>
            command?.SetTag(CompletedOnUiThreadTag, dispatcher.CheckAccess()));

        return result;
    }

    /// <summary>
    /// Stand-in for a persistence layer: opens a <see cref="WorkSpanName"/> span of its
    /// own, knowing nothing about the command that called it.
    /// </summary>
    public static Task<string> PersistAsync()
    {
        // Knows nothing about the command, and ends up inside it anyway.
        using var activity = Source.StartActivity(WorkSpanName);

        return Task.FromResult("saved");
    }
}
