using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using OpenTelemetry;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex030_SamplersTests
{
    private static (Activity? Activity, int Exported) Run(Sampler sampler, ActivityContext parent = default)
    {
        var exported = new List<Activity>();

        using var provider = Ex030_Samplers.Build(exported, sampler);
        var activity = Ex030_Samplers.DoWork(parent);
        provider.ForceFlush();

        return (activity, exported.Count);
    }

    [Fact]
    public void AlwaysOn_records_and_exports()
    {
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(new AlwaysOnSampler());

        Assert.NotNull(activity);
        Assert.True(activity.Recorded);
        Assert.True(activity.IsAllDataRequested);
        Assert.Equal(1, exported);
    }

    [Fact]
    public void Adversarial_A_AlwaysOff_still_produces_an_activity_it_simply_does_not_record()
    {
        // Row 015's finding one level up, and the whole reason sampling can work at all.
        // A dropped span is not absent: it exists, it carries a trace id and a span id,
        // and it propagates - so a downstream service still knows which trace it belongs
        // to and can make the same decision. What it does not do is record.
        //
        // That is why `if (activity.IsAllDataRequested)` matters on the hot path: at a
        // 1% sample rate, 99% of your spans are this shape, and any unguarded tagging is
        // work thrown away 99 times out of 100.
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(new AlwaysOffSampler());

        Assert.NotNull(activity);
        Assert.False(activity.Recorded);
        Assert.False(activity.IsAllDataRequested);
        Assert.Equal(0, exported);
        Assert.NotEqual(default, activity.TraceId);
    }

    [Theory]
    [InlineData(0.0, false)]
    [InlineData(1.0, true)]
    public void A_ratio_of_zero_behaves_as_off_and_one_as_on(double ratio, bool expected)
    {
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(new TraceIdRatioBasedSampler(ratio));

        Assert.NotNull(activity);
        Assert.Equal(expected, activity.Recorded);
        Assert.Equal(expected ? 1 : 0, exported);
    }

    [Fact]
    public void Adversarial_B_ParentBased_honours_a_sampled_remote_parent_over_its_own_default()
    {
        // What keeps a trace whole. If every service sampled independently at 10%, a
        // five-hop trace would survive end to end one time in a hundred thousand, and
        // the ones you did keep would be full of holes.
        //
        // ParentBased says: whoever started this trace already decided - honour it. Here
        // the root behaviour is AlwaysOff and the parent says sampled, so the span is
        // recorded anyway.
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(
            new ParentBasedSampler(new AlwaysOffSampler()),
            Ex030_Samplers.RemoteParent(sampled: true));

        Assert.NotNull(activity);
        Assert.True(activity.Recorded);
        Assert.Equal(1, exported);
    }

    [Fact]
    public void Adversarial_C_ParentBased_honours_an_unsampled_remote_parent_too()
    {
        // The matched half, and the direction people forget. A ParentBased sampler whose
        // root is AlwaysOn but which ignores an unsampled parent quietly records
        // everything the caller asked to drop - so the sample rate the head chose stops
        // meaning anything from the second hop onward.
        using var ctx = new TelemetryContext();

        var (activity, exported) = Run(
            new ParentBasedSampler(new AlwaysOnSampler()),
            Ex030_Samplers.RemoteParent(sampled: false));

        Assert.NotNull(activity);
        Assert.False(activity.Recorded);
        Assert.Equal(0, exported);
    }

    [Fact]
    public void The_remote_parent_is_a_remote_parent()
    {
        // Keeps the two facts above honest: ParentBased treats remote and local parents
        // through different branches, and a context built without IsRemote would be
        // exercising the wrong one.
        using var ctx = new TelemetryContext();

        var parent = Ex030_Samplers.RemoteParent(sampled: true);

        Assert.True(parent.IsRemote);
        Assert.Equal(ActivityTraceFlags.Recorded, parent.TraceFlags);
        Assert.NotEqual(default, parent.TraceId);
    }
}
