using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 065 — FlushOnShutdown (desktop-ops).
// Goal:   Know the size of what you stand to lose, instead of finding out afterwards.
// Drills: a processor that counts what has not left yet, flushing on exit, the loss
//         window as a number.
// Passes: finished work that has not been flushed raises Pending;
//         flushing delivers it and returns Pending to zero;
//         a process that exits without flushing loses exactly Pending records - no more
//                     and no fewer;
//         Shutdown after a flush delivers nothing further, because nothing is left;
//         and Pending is zero before anything has happened, rather than unknown.
//
// Row 044 established that a batch processor has a window in which telemetry exists in one
// process's memory and nowhere else. This row makes that window a number the application
// can see.
//
// That matters far more on a desktop than on a server. A server is stopped by an
// orchestrator that sends a signal and waits; a desktop application is closed by a person
// clicking an X, killed by a laptop lid, or ended by an update that decided now was a good
// time. There is no graceful path you can rely on - so the useful question is not "did we
// flush" but "how much would we have lost", and the only way to answer it is to count.
//
// The third clause is the one that turns the count into something worth having. If Pending
// is usually 3, an ungraceful exit costs three records and nobody needs to care. If it is
// usually 4000 because the schedule is five minutes, every crash loses five minutes of
// evidence about the crash - and the fix is a shorter schedule, which you would never have
// known to make.
public static class Ex065_FlushOnShutdown
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex065";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "work";

    /// <summary>
    /// Long enough that the scheduled export never fires during a test, so the only thing
    /// that can move a record is an explicit flush.
    /// </summary>
    public const int ScheduleDelayMilliseconds = 600_000;

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// How many finished spans have not yet reached <c>delivered</c>.
    ///
    /// Reset by <see cref="Build"/>, so each pipeline starts from a known zero rather than
    /// from whatever the last one left behind.
    /// </summary>
    private static int pending;

    public static int Pending => Volatile.Read(ref pending);

    /// <summary>
    /// Build a provider listening to <see cref="SourceName"/> whose exporter sits behind a
    /// batch processor with a schedule delay of
    /// <see cref="ScheduleDelayMilliseconds"/>, and which counts every finished span as
    /// pending until it is actually delivered into <paramref name="delivered"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> delivered)
    {
        // Reset, so each pipeline starts from a known zero rather than from whatever the
        // last one left behind.
        Volatile.Write(ref pending, 0);

        return Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            // Counts on the way in, at the moment a span finishes and joins the queue.
            .AddProcessor(new PendingCounter())
            .AddProcessor(new BatchActivityExportProcessor(
                new CountingExporter(delivered),
                scheduledDelayMilliseconds: ScheduleDelayMilliseconds))
            .Build();
    }

    /// <summary>Every finished span is pending until something delivers it.</summary>
    private sealed class PendingCounter : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity data) => Interlocked.Increment(ref pending);
    }

    /// <summary>And stops being pending at the moment it actually leaves.</summary>
    private sealed class CountingExporter(ICollection<Activity> delivered) : BaseExporter<Activity>
    {
        public override ExportResult Export(in Batch<Activity> batch)
        {
            foreach (var activity in batch)
            {
                delivered.Add(activity);
                Interlocked.Decrement(ref pending);
            }

            return ExportResult.Success;
        }
    }

    /// <summary>Start and stop one <see cref="WorkSpanName"/> span.</summary>
    public static void DoWork()
    {
        using var activity = Source.StartActivity(WorkSpanName);
    }
}
