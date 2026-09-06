using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex038_ContextPropagatorsTests
{
    private static ActivityContext SomeContext() =>
        new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);

    [Fact]
    public void Inject_writes_a_well_formed_traceparent()
    {
        using var ctx = new TelemetryContext();
        var context = SomeContext();

        var carrier = Ex038_ContextPropagators.Inject(new PropagationContext(context, default));

        var header = Assert.Contains(Ex038_ContextPropagators.TraceParentHeader, carrier);
        Assert.StartsWith("00-", header);
        Assert.Contains(context.TraceId.ToHexString(), header);
        Assert.Contains(context.SpanId.ToHexString(), header);
        Assert.EndsWith("-01", header);
    }

    [Fact]
    public void The_context_round_trips_and_comes_back_marked_remote()
    {
        using var ctx = new TelemetryContext();
        var context = SomeContext();

        var extracted = Ex038_ContextPropagators.Extract(
            (IReadOnlyDictionary<string, string>)Ex038_ContextPropagators.Inject(
                new PropagationContext(context, default)));

        Assert.Equal(context.TraceId, extracted.ActivityContext.TraceId);
        Assert.Equal(context.SpanId, extracted.ActivityContext.SpanId);
        Assert.Equal(context.TraceFlags, extracted.ActivityContext.TraceFlags);

        // Remote, because it came off a wire. ParentBased samplers branch on this.
        Assert.True(extracted.ActivityContext.IsRemote);
    }

    [Fact]
    public void Adversarial_A_The_same_pair_carries_baggage_in_its_own_header()
    {
        // Why the SDK's default is a COMPOSITE rather than the trace propagator alone.
        // Two propagators, two independent headers, one call - and a hand-rolled injector
        // that only writes traceparent drops every piece of baggage at that boundary
        // silently. Nothing downstream can tell "no baggage was set" from "the hop threw
        // it away".
        using var ctx = new TelemetryContext();
        var baggage = Baggage.Create(new Dictionary<string, string> { ["tenant.id"] = "acme" });

        var carrier = Ex038_ContextPropagators.Inject(new PropagationContext(SomeContext(), baggage));

        var header = Assert.Contains(Ex038_ContextPropagators.BaggageHeader, carrier);
        Assert.Contains("tenant.id", header);
        Assert.Contains("acme", header);
    }

    [Fact]
    public void Adversarial_B_The_baggage_survives_the_round_trip_too()
    {
        // The paired half. A propagator that writes the header and cannot read it back is
        // half a seam, and the failure only shows up on the receiving service.
        using var ctx = new TelemetryContext();
        var baggage = Baggage.Create(new Dictionary<string, string> { ["tenant.id"] = "acme" });

        var extracted = Ex038_ContextPropagators.Extract(
            (IReadOnlyDictionary<string, string>)Ex038_ContextPropagators.Inject(
                new PropagationContext(SomeContext(), baggage)));

        Assert.Equal("acme", extracted.Baggage.GetBaggage("tenant.id"));
    }

    [Fact]
    public void Adversarial_C_An_empty_carrier_yields_an_empty_context()
    {
        // Row 020's malformed-header lesson one level up: this carrier came from outside.
        // A missing header is NORMAL - it is what the first hop of every trace looks
        // like - and the correct answer is an empty context, from which the SDK starts a
        // fresh trace.
        using var ctx = new TelemetryContext();

        var extracted = Ex038_ContextPropagators.Extract(new Dictionary<string, string>());

        Assert.Equal(default, extracted.ActivityContext);
    }
}
