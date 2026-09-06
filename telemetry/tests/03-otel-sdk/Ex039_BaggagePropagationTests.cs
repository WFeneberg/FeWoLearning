using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex039_BaggagePropagationTests
{
    private static (Activity? Span, string? Tenant) Hop(bool copyOntoSpan)
    {
        var exported = new List<Activity>();

        using var provider = Ex039_BaggagePropagation.BuildTracing(exported);
        var carrier = Ex039_BaggagePropagation.CallerSide("acme");

        return Ex039_BaggagePropagation.CalleeSide(
            (IReadOnlyDictionary<string, string>)carrier, copyOntoSpan);
    }

    [Fact]
    public void The_carrier_names_the_tenant()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex039_BaggagePropagation.BuildTracing(exported);
        var carrier = Ex039_BaggagePropagation.CallerSide("acme");

        var header = Assert.Contains("baggage", carrier);
        Assert.Contains(Ex039_BaggagePropagation.TenantBaggageKey, header);
        Assert.Contains("acme", header);
    }

    [Fact]
    public void The_tenant_is_readable_on_the_other_side()
    {
        using var ctx = new TelemetryContext();

        var (_, tenant) = Hop(copyOntoSpan: false);

        Assert.Equal("acme", tenant);
    }

    [Fact]
    public void Adversarial_A_Arriving_is_not_being_recorded()
    {
        // The thing people get wrong about baggage. It is CONTEXT, not data: it travels,
        // it is available to every frame under it and to every service downstream - and
        // no backend indexes it, no dashboard filters on it, and no span carries it.
        //
        // A value that is in baggage and nowhere else is invisible to everything except
        // code that deliberately reads it. The tenant is demonstrably present here, and
        // the span still says nothing about it.
        using var ctx = new TelemetryContext();

        var (span, tenant) = Hop(copyOntoSpan: false);

        Assert.Equal("acme", tenant);
        Assert.NotNull(span);
        Assert.Null(span.GetTagItem(Ex039_BaggagePropagation.TenantAttribute));
    }

    [Fact]
    public void Adversarial_B_Copying_it_onto_the_span_is_what_records_it()
    {
        // The matched half, and the pattern: propagate in baggage, record on the span at
        // the point where it matters. That copy is a decision, not an oversight - it is
        // where you choose which services pay for the attribute, and which of them are
        // allowed to write a tenant id into permanent storage at all.
        using var ctx = new TelemetryContext();

        var (span, _) = Hop(copyOntoSpan: true);

        Assert.NotNull(span);
        Assert.Equal("acme", span.GetTagItem(Ex039_BaggagePropagation.TenantAttribute)?.ToString());
    }

    [Fact]
    public void Adversarial_C_The_caller_leaves_no_baggage_behind_on_its_own_thread()
    {
        // Baggage is ambient and it is AsyncLocal, so a caller that sets it and walks
        // away has attached the tenant to everything that thread does next - including
        // the next request it picks up, and every outbound call that request makes.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex039_BaggagePropagation.BuildTracing(exported);
        Ex039_BaggagePropagation.CallerSide("acme");

        Assert.Null(OpenTelemetry.Baggage.Current.GetBaggage(Ex039_BaggagePropagation.TenantBaggageKey));
    }
}
