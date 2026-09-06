using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex070_GracefulShutdownFlushInContainerTests
{
    /// <summary>
    /// An exporter that does not come back until it is told to - a collector that is down,
    /// or a network that is swallowing the connection. Nothing else in the SDK models
    /// "slow", and the deadline is untestable without it.
    /// </summary>
    private sealed class BlockingExporter : BaseExporter<Activity>
    {
        private readonly ManualResetEventSlim _release = new(initialState: false);

        public override ExportResult Export(in Batch<Activity> batch)
        {
            _release.Wait(TimeSpan.FromSeconds(30));
            return ExportResult.Success;
        }

        public void Release() => _release.Set();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _release.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A hosted service that marks the phase BETWEEN ApplicationStopping and
    /// ApplicationStopped, which is the only phase-based way to tell the two apart.
    ///
    /// A second callback on ApplicationStopped would not do it: cancellation-token
    /// callbacks run last-registered-first, so a wrong implementation registering on
    /// Stopped after the test did still appears to run "first". Measured - the earlier
    /// version of this fact passed against exactly that wrong implementation.
    /// </summary>
    private sealed class StopMarker(IList<string> log) : IHostedService
    {
        public const string Marker = "hosted-service-stopped";

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.Add(Marker);
            return Task.CompletedTask;
        }
    }

    private static IHost StartedHost(IList<string>? stopMarkerLog = null)
    {
        var builder = Host.CreateApplicationBuilder();

        if (stopMarkerLog is not null)
            builder.Services.AddSingleton<IHostedService>(new StopMarker(stopMarkerLog));

        var host = builder.Build();
        host.Start();
        return host;
    }

    private static IHostApplicationLifetime LifetimeOf(IHost host) =>
        host.Services.GetRequiredService<IHostApplicationLifetime>();

    [Fact]
    public async Task The_work_finished_before_shutdown_is_delivered_when_stopping_begins()
    {
        using var ctx = new TelemetryContext();
        using var host = StartedHost();

        var delivered = new List<Activity>();
        var log = new List<string>();

        using var provider = Ex070_GracefulShutdownFlushInContainer.Build(delivered);
        Ex070_GracefulShutdownFlushInContainer.FlushOnStopping(
            LifetimeOf(host), provider, TimeSpan.FromSeconds(5), log);

        Ex070_GracefulShutdownFlushInContainer.DoWork();
        Assert.Empty(delivered);

        await host.StopAsync();

        var span = Assert.Single(delivered);
        Assert.Equal(Ex070_GracefulShutdownFlushInContainer.WorkSpanName, span.DisplayName);
        Assert.Equal(
            new[]
            {
                Ex070_GracefulShutdownFlushInContainer.FlushedMarker,
                Ex070_GracefulShutdownFlushInContainer.StoppedMarker,
            },
            log);
    }

    [Fact]
    public async Task Adversarial_A_Without_the_hook_the_last_span_is_simply_lost()
    {
        // The paired half. The batch processor's schedule is ten minutes here, so nothing
        // leaves on its own and the span is still sitting in the queue when the process
        // would have been killed. This is what the hook is for, and it is the fact that
        // stops "the span arrived" from being satisfied by an exporter that would have
        // exported anyway.
        using var ctx = new TelemetryContext();
        using var host = StartedHost();

        var delivered = new List<Activity>();

        using var provider = Ex070_GracefulShutdownFlushInContainer.Build(delivered);

        Ex070_GracefulShutdownFlushInContainer.DoWork();
        await host.StopAsync();

        Assert.Empty(delivered);
    }

    [Fact]
    public async Task Adversarial_B_The_flush_happens_while_stopping_not_after_stopped()
    {
        // ApplicationStopped fires once every hosted service has already stopped and the
        // process is on its way out - registering there is the plausible wrong answer,
        // because on a host that is being killed the callback may never run at all.
        //
        // This grades the ORDER, which is the only observable difference between the two
        // registrations on a host that shuts down cleanly.
        using var ctx = new TelemetryContext();

        var delivered = new List<Activity>();
        var log = new List<string>();

        using var host = StartedHost(log);

        using var provider = Ex070_GracefulShutdownFlushInContainer.Build(delivered);
        Ex070_GracefulShutdownFlushInContainer.FlushOnStopping(
            LifetimeOf(host), provider, TimeSpan.FromSeconds(5), log);

        Ex070_GracefulShutdownFlushInContainer.DoWork();
        await host.StopAsync();

        // Stopping runs first, then every hosted service stops, and only then does
        // Stopped fire - so a flush registered on Stopping lands BEFORE the marker and one
        // registered on Stopped lands after it.
        Assert.Equal(
            new[]
            {
                Ex070_GracefulShutdownFlushInContainer.FlushedMarker,
                Ex070_GracefulShutdownFlushInContainer.StoppedMarker,
                StopMarker.Marker,
            },
            log);
    }

    [Fact]
    public async Task Adversarial_C_A_flush_that_cannot_finish_gives_up_at_its_deadline()
    {
        // The clause that turns a good idea into an outage. The orchestrator sends SIGTERM
        // and waits a fixed grace period before SIGKILL. A flush with no deadline against a
        // collector that is down does not fail - it waits. So the process ignores its stop
        // signal, is killed anyway, loses the telemetry it was trying to save AND spends the
        // whole grace period per replica doing it.
        //
        // Note what is asserted and what is not: that it gave up, that it SAID so, and that
        // shutdown continued regardless. Telemetry is never worth delaying a stop for.
        using var ctx = new TelemetryContext();
        using var host = StartedHost();

        var exporter = new BlockingExporter();
        var log = new List<string>();
        var deadline = TimeSpan.FromMilliseconds(750);

        var provider = Sdk.CreateTracerProviderBuilder()
            .AddSource(Ex070_GracefulShutdownFlushInContainer.SourceName)
            .AddProcessor(new BatchActivityExportProcessor(
                exporter,
                scheduledDelayMilliseconds:
                    Ex070_GracefulShutdownFlushInContainer.ScheduleDelayMilliseconds))
            .Build();

        try
        {
            Ex070_GracefulShutdownFlushInContainer.FlushOnStopping(
                LifetimeOf(host), provider, deadline, log);
            Ex070_GracefulShutdownFlushInContainer.DoWork();

            var stopwatch = Stopwatch.StartNew();
            await host.StopAsync();
            stopwatch.Stop();

            Assert.Equal(
                new[]
                {
                    Ex070_GracefulShutdownFlushInContainer.TimedOutMarker,
                    Ex070_GracefulShutdownFlushInContainer.StoppedMarker,
                },
                log);

            // Comfortably inside the grace period, which is the whole point of the deadline.
            Assert.True(
                stopwatch.Elapsed < TimeSpan.FromSeconds(
                    Ex070_GracefulShutdownFlushInContainer.GraceSeconds),
                $"shutdown took {stopwatch.Elapsed}, which a {Ex070_GracefulShutdownFlushInContainer.GraceSeconds}s grace period would not have survived");
        }
        finally
        {
            exporter.Release();
            provider.Dispose();
        }
    }

    [Fact]
    public async Task Container_A_stopped_container_receives_SIGTERM_before_it_is_killed()
    {
        // 🐳 Everything above assumes the process gets told. This is the assumption itself,
        // asked of a real container: the handler runs, prints, and exits inside the grace
        // period - so ApplicationStopping really does have a window to flush in, and it
        // really is bounded.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new ContainerBuilder("alpine:3.21")
            .WithEntrypoint(
                "/bin/sh",
                "-c",
                $"trap 'echo {Ex070_GracefulShutdownFlushInContainer.FlushedMarker}; exit 0' TERM; "
                + "echo ready; while true; do sleep 0.2; done")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("ready"))
            .Build();

        await container.StartAsync();

        var stopwatch = Stopwatch.StartNew();
        await container.StopAsync();
        stopwatch.Stop();

        var (stdout, stderr) = await container.GetLogsAsync();

        Assert.Contains(
            Ex070_GracefulShutdownFlushInContainer.FlushedMarker, stdout + stderr);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(
                Ex070_GracefulShutdownFlushInContainer.GraceSeconds),
            $"the handler did not finish inside the grace period: {stopwatch.Elapsed}");
    }
}
