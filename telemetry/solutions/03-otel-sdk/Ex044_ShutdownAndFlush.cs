using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 044 — ShutdownAndFlush (otel-sdk).
// Goal:   Find the window in which your telemetry exists and is not yet anywhere, and
//         learn what closes it.
// Drills: BatchExportProcessor, ForceFlush, Shutdown, disposal order.
// Passes: with a batch processor and a long schedule, a finished span is NOT exported;
//         ForceFlush exports it;
//         after Shutdown, later spans are never exported, and Shutdown is not undone by
//                     a second call;
//         and a span that is still OPEN when the provider goes away is lost entirely.
//
// The first clause is the window, and it is the point of a batch processor: exporting one
// span per operation would put a network call on every request, so the SDK buffers and
// ships in batches. Between the end of a span and the next batch, that span exists in one
// process's memory and nowhere else. Kill the process there - a crash, a SIGKILL, a
// scale-in - and it is simply gone.
//
// Which is why the last clause is the one that bites at shutdown. The spans you most want
// are the ones from the last seconds of a process that was about to die, and those are
// exactly the ones sitting in the buffer. A host that disposes its provider before its
// background work has finished loses them, and a host that never disposes it loses them
// too. ForceFlush before exit, Shutdown after, in that order.
//
// The third clause is the part people are surprised by: Shutdown is FINAL. It is not a
// pause and there is no restart - a provider that has been shut down accepts nothing more
// for the life of the process. Calling it early "to be safe" silently ends telemetry.
public static class Ex044_ShutdownAndFlush
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex044";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "work";

    /// <summary>
    /// Long enough that the scheduled export never fires during a test, so the only
    /// thing that can flush the batch is an explicit call.
    /// </summary>
    public const int ScheduleDelayMilliseconds = 600_000;

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider listening to <see cref="SourceName"/> whose exporter sits behind
    /// a BATCH processor with a schedule delay of
    /// <see cref="ScheduleDelayMilliseconds"/> - so nothing leaves on its own.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider BuildBatched(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            // AddInMemoryExporter has no options overload for traces, so the processor is
            // built by hand - which is no bad thing here, since the processor IS the
            // subject of the row.
            .AddProcessor(new BatchActivityExportProcessor(
                new InMemoryExporter<Activity>(exported),
                scheduledDelayMilliseconds: ScheduleDelayMilliseconds))
            .Build();

    /// <summary>Start and stop one <see cref="WorkSpanName"/> span, and return it.</summary>
    public static Activity? DoWork()
    {
        using var activity = Source.StartActivity(WorkSpanName);

        return activity;
    }

    /// <summary>
    /// Start a span, leave it OPEN, and return it. The caller decides what happens next -
    /// which is the point.
    /// </summary>
    public static Activity? StartUnfinishedWork() => Source.StartActivity(WorkSpanName);
}
