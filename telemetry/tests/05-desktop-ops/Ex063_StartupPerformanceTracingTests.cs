using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex063_StartupPerformanceTracingTests
{
    private static async Task<(List<Activity> Spans, Exception? Failure)> RunStartup(
        params (string Name, Func<Task> Work)[] phases)
    {
        var exported = new List<Activity>();
        Exception? failure = null;

        using var provider = Ex063_StartupPerformanceTracing.Build(exported);

        try
        {
            await Ex063_StartupPerformanceTracing.RunStartupAsync(phases);
        }
        catch (Exception thrown)
        {
            failure = thrown;
        }

        provider.ForceFlush();

        return (exported, failure);
    }

    private static (string, Func<Task>) Phase(string name) => (name, () => Task.CompletedTask);

    [Fact]
    public async Task Each_phase_gets_its_own_span_under_one_root()
    {
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunStartup(Phase("configuration"), Phase("services"), Phase("shell"));

        var root = Assert.Single(spans, s => s.DisplayName == Ex063_StartupPerformanceTracing.StartupSpanName);
        var phases = spans.Where(s => s != root).ToArray();

        Assert.Equal(3, phases.Length);
        Assert.Equal(
            ["configuration", "services", "shell"],
            phases.Select(s => s.GetTagItem(Ex063_StartupPerformanceTracing.PhaseTag)?.ToString()));
    }

    [Fact]
    public async Task Adversarial_A_The_phases_are_siblings_and_the_root_stops_last()
    {
        // The shape row 016 taught, arriving where it changes what a waterfall says. A
        // chain would claim services waited for configuration to finish for a reason;
        // siblings say only that they ran in that order.
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunStartup(Phase("configuration"), Phase("services"));

        var root = Assert.Single(spans, s => s.DisplayName == Ex063_StartupPerformanceTracing.StartupSpanName);
        var phases = spans.Where(s => s != root).ToArray();

        Assert.All(phases, s => Assert.Equal(root.SpanId, s.ParentSpanId));

        // Children finish before their parent, so the root is exported last.
        Assert.Same(root, spans[^1]);
    }

    [Fact]
    public async Task Adversarial_B_A_failing_phase_stops_startup_and_says_so()
    {
        // Row 018's lesson at the least forgiving moment. A startup that swallows a failed
        // phase produces an application that opens into a broken state - and telemetry
        // that, having recorded a green root span, agrees everything is fine.
        using var ctx = new TelemetryContext();

        var (spans, failure) = await RunStartup(
            Phase("configuration"),
            ("services", () => Task.FromException(new InvalidOperationException("no container"))),
            Phase("shell"));

        Assert.IsType<InvalidOperationException>(failure);

        var root = Assert.Single(spans, s => s.DisplayName == Ex063_StartupPerformanceTracing.StartupSpanName);
        Assert.Equal(ActivityStatusCode.Error, root.Status);

        var services = Assert.Single(
            spans,
            s => s.GetTagItem(Ex063_StartupPerformanceTracing.PhaseTag)?.ToString() == "services");
        Assert.Equal(ActivityStatusCode.Error, services.Status);
    }

    [Fact]
    public async Task Adversarial_C_No_phase_after_the_failing_one_runs()
    {
        // The paired half. A startup that carries on past a broken phase has turned a
        // clean failure into a half-initialised application, which is the harder bug by a
        // wide margin.
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunStartup(
            Phase("configuration"),
            ("services", () => Task.FromException(new InvalidOperationException("no container"))),
            Phase("shell"));

        Assert.DoesNotContain(
            spans,
            s => s.GetTagItem(Ex063_StartupPerformanceTracing.PhaseTag)?.ToString() == "shell");
    }

    [Fact]
    public async Task A_successful_startup_leaves_every_span_unmarked()
    {
        using var ctx = new TelemetryContext();

        var (spans, failure) = await RunStartup(Phase("configuration"), Phase("services"));

        Assert.Null(failure);
        Assert.All(spans, s => Assert.NotEqual(ActivityStatusCode.Error, s.Status));
    }
}
