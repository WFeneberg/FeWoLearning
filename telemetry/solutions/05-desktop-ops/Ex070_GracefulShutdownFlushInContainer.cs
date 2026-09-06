using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 070 — GracefulShutdownFlushInContainer (desktop-ops). 🐳
// Goal:   Get the last few seconds of telemetry out of a process that is being stopped,
//         and not hang the stop trying.
// Drills: IHostApplicationLifetime, flushing on ApplicationStopping, a flush DEADLINE.
// Passes: work finished before shutdown is delivered when stopping begins;
//         without the hook it is lost;
//         the flush happens while stopping, not after stopped;
//         a flush that cannot finish gives up at its deadline and says so, rather than
//                     holding the process open;
//         and 🐳 a real container receives SIGTERM on stop, so a handler genuinely gets
//                     the chance to run.
//
// This is the last row of the track and it closes the circle rows 044 and 065 opened. The
// window exists; it has a size; and this is where you get to empty it - once, at the only
// moment you know is coming.
//
// The fourth clause is the one that turns a good idea into an outage. An orchestrator
// sends SIGTERM and then waits a fixed grace period - ten seconds by default for Docker -
// before sending SIGKILL. A flush with no deadline against a collector that is itself
// down does not fail; it waits. So the process ignores its stop signal, gets killed
// anyway, loses the telemetry it was trying to save AND takes ten seconds per replica
// doing it, which during a rolling deploy is the difference between a minute and an hour.
//
// The deadline has to be shorter than the grace period, and what happens when it expires
// has to be "record that it did and carry on". Telemetry is never worth delaying a
// shutdown for.
public static class Ex070_GracefulShutdownFlushInContainer
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex070";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "work";

    /// <summary>Docker's default grace period. The flush deadline must be shorter.</summary>
    public const int GraceSeconds = 10;

    /// <summary>What the hook writes when everything got out.</summary>
    public const string FlushedMarker = "flushed";

    /// <summary>What it writes when the deadline expired first.</summary>
    public const string TimedOutMarker = "flush-timed-out";

    /// <summary>What it writes once shutdown may proceed.</summary>
    public const string StoppedMarker = "stopped";

    /// <summary>
    /// Long enough that the scheduled export never fires on its own, so the only thing
    /// that can move a record is the hook under test.
    /// </summary>
    public const int ScheduleDelayMilliseconds = 600_000;

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider listening to <see cref="SourceName"/> whose exporter sits behind a
    /// batch processor slow enough that nothing leaves by itself.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> delivered) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .AddProcessor(new BatchActivityExportProcessor(
                new InMemoryExporter<Activity>(delivered),
                scheduledDelayMilliseconds: ScheduleDelayMilliseconds))
            .Build();

    /// <summary>Start and stop one <see cref="WorkSpanName"/> span.</summary>
    public static void DoWork()
    {
        using var span = Source.StartActivity(WorkSpanName);
    }

    /// <summary>
    /// Register on <paramref name="lifetime"/>'s <c>ApplicationStopping</c> so that
    /// <paramref name="provider"/> is flushed with <paramref name="deadline"/> as its
    /// limit.
    ///
    /// Append <see cref="FlushedMarker"/> to <paramref name="log"/> when the flush
    /// completed, <see cref="TimedOutMarker"/> when the deadline expired first - and in
    /// either case <see cref="StoppedMarker"/> afterwards, because shutdown proceeds
    /// regardless. Telemetry is never worth delaying a stop for.
    /// </summary>
    public static void FlushOnStopping(
        IHostApplicationLifetime lifetime,
        TracerProvider provider,
        TimeSpan deadline,
        IList<string> log) =>
        // ApplicationStopping, not ApplicationStopped. Stopping runs while the host is
        // still taking itself down and there is something to flush; Stopped runs after
        // every hosted service has already gone - and on a host that is being killed it
        // may not run at all.
        lifetime.ApplicationStopping.Register(() =>
        {
            // ForceFlush's overload takes the deadline and RETURNS whether it made it.
            // Calling the argument-less overload instead is the outage: it waits forever
            // against a collector that is down, the process ignores its stop signal, gets
            // SIGKILLed at the end of the grace period anyway, and loses the telemetry it
            // was holding the shutdown open for.
            var flushed = provider.ForceFlush((int)deadline.TotalMilliseconds);

            log.Add(flushed ? FlushedMarker : TimedOutMarker);

            // Unconditionally, on both paths. Telemetry is never worth delaying a
            // shutdown for, so "we did not manage it" is a thing to record and carry on
            // from, not a thing to retry.
            log.Add(StoppedMarker);
        });
}
