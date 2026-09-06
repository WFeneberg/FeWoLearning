using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex031_CustomSamplerTests
{
    private static (Activity? Activity, int Exported) Run(string route, string? routeAfterStart = null)
    {
        var exported = new List<Activity>();

        using var provider = Ex031_CustomSampler.Build(exported);
        var activity = Ex031_CustomSampler.DoWork(route, routeAfterStart);
        provider.ForceFlush();

        return (activity, exported.Count);
    }

    [Fact]
    public void A_business_route_is_recorded_and_exported()
    {
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run("/orders");

        Assert.NotNull(activity);
        Assert.True(activity.Recorded);
        Assert.True(activity.IsAllDataRequested);
        Assert.Equal(1, exported);
    }

    [Fact]
    public void The_health_route_is_dropped()
    {
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(Ex031_CustomSampler.HealthRoute);

        Assert.NotNull(activity);
        Assert.False(activity.Recorded);
        Assert.False(activity.IsAllDataRequested);
        Assert.Equal(0, exported);
    }

    [Fact]
    public void Adversarial_A_RecordOnly_builds_the_span_and_still_never_exports_it()
    {
        // The clause worth the exercise, and the one a two-way sampler cannot express.
        // RecordOnly is not "half sampled": the span is fully populated, so a processor
        // can read it and turn it into a metric or a log - but Recorded is false, so the
        // exporter skips it and every downstream service sees an unsampled traceparent.
        //
        // A Drop and a RecordOnly are indistinguishable if you only look at the export
        // count. IsAllDataRequested is what separates them.
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(Ex031_CustomSampler.CacheRoute);

        Assert.NotNull(activity);
        Assert.True(activity.IsAllDataRequested, "RecordOnly must still request all data");
        Assert.False(activity.Recorded, "RecordOnly must not set the sampled flag");
        Assert.Equal(0, exported);
    }

    [Fact]
    public void Adversarial_B_A_tag_set_after_the_span_started_cannot_change_the_decision()
    {
        // The limitation everyone hits once. A sampler runs BEFORE the span exists -
        // that is the point, since its answer decides whether the span gets built at
        // all - so it sees only the attributes passed to StartActivity.
        //
        // Deciding on something you learn later (a status code, a duration, a user's
        // plan) is not possible here. That is what tail sampling in a collector is for.
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run("/orders", routeAfterStart: Ex031_CustomSampler.HealthRoute);

        Assert.NotNull(activity);
        Assert.Equal(Ex031_CustomSampler.HealthRoute,
            activity.GetTagItem(Ex031_CustomSampler.RouteTag)?.ToString());
        Assert.True(activity.Recorded, "the decision was made from the tag present at start");
        Assert.Equal(1, exported);
    }

    [Fact]
    public void Adversarial_C_The_decision_reads_the_route_and_not_the_span_name()
    {
        // Every span here is called "request", so a sampler keying on the name could
        // only ever answer one way. This fact fails against exactly that implementation
        // while the three above would not notice.
        using var ctx = new TelemetryContext();

        var health = Run(Ex031_CustomSampler.HealthRoute);
        var orders = Run("/orders");

        Assert.Equal(Ex031_CustomSampler.WorkSpanName, health.Activity?.DisplayName);
        Assert.Equal(Ex031_CustomSampler.WorkSpanName, orders.Activity?.DisplayName);
        Assert.NotEqual(health.Exported, orders.Exported);
    }
}
