using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Support;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Canaries. These must pass in BOTH modes (`dotnet test` and
/// `dotnet test -p:UseSolutions=true`) and are the first thing to fail when a
/// package bump breaks the harness. They are never TODOs and never get a
/// catalog.md row.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void The_test_project_references_exactly_one_content_library()
    {
        // Touch a type from the content library FIRST. A referenced assembly is
        // loaded lazily, so walking GetAssemblies() without this finds nothing and
        // the canary fails for a reason unrelated to the track. Measured 2026-09-06.
        Assert.Equal("telemetry", TrackMarker.TrackName);

        var loaded = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .Where(n => n is "FeWoLearning.Telemetry.Exercises" or "FeWoLearning.Telemetry.Solutions")
            .ToArray();

        // Two would mean the UseSolutions switch stopped being exclusive, and the
        // identical type names would collide. Zero is impossible after the line above.
        Assert.Single(loaded);
    }

    [Fact]
    public void TraceProbe_sees_only_its_own_source()
    {
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe("harness.smoke.mine");
        using var mine = new ActivitySource("harness.smoke.mine");
        using var other = new ActivitySource("harness.smoke.other");

        using (mine.StartActivity("kept")) { }
        using (other.StartActivity("ignored")) { }

        Assert.Equal("kept", probe.Single().DisplayName);
    }

    [Fact]
    public void TelemetryContext_clears_the_ambient_activity()
    {
        using var source = new ActivitySource("harness.smoke.ambient");
        using var probe = new TraceProbe("harness.smoke.ambient");
        var leaked = source.StartActivity("leaked");
        Assert.NotNull(Activity.Current);

        using var ctx = new TelemetryContext();

        Assert.Null(Activity.Current);
        leaked?.Dispose();
    }

    [Fact]
    public void LogProbe_exposes_named_fields_not_just_the_message()
    {
        using var logs = new LogProbe();

        logs.For("harness").LogInformation("order {OrderId} shipped", "O-7");

        var record = Assert.Single(logs.Records);
        Assert.Equal("O-7", LogProbe.Field(record, "OrderId"));
    }

    [Fact]
    public void MetricProbe_collects_a_counter_from_its_own_meter()
    {
        using var probe = new MetricProbe("harness.smoke.meter");
        using var meter = new System.Diagnostics.Metrics.Meter("harness.smoke.meter");
        meter.CreateCounter<long>("hits").Add(3);

        var point = Assert.Single(probe.Collect());
        Assert.Equal("hits", point.Instrument);
        Assert.Equal(3d, point.Sum);
    }

    [Fact]
    public void ContainerGate_skips_by_default_and_runs_under_the_flag()
    {
        ContainerGate.SkipUnlessEnabled();

        // Only reached under -p:Containers=true. If this line ever runs on a plain
        // `dotnet test`, the gate is broken and every container fact is silently
        // executing (or silently passing) in the default run.
        Assert.True(ContainerGate.Enabled);
    }
}
