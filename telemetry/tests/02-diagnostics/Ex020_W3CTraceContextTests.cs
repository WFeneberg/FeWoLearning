using System.Diagnostics;
using System.Text.RegularExpressions;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex020_W3CTraceContextTests
{
    private static ActivityContext Context(ActivityTraceFlags flags) =>
        new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), flags);

    [Fact]
    public void The_header_has_exactly_the_shape_the_spec_requires()
    {
        using var ctx = new TelemetryContext();
        var context = Context(ActivityTraceFlags.Recorded);

        var header = Ex020_W3CTraceContext.FormatTraceParent(context);

        Assert.Matches(new Regex("^00-[0-9a-f]{32}-[0-9a-f]{16}-[0-9a-f]{2}$"), header);
        Assert.Equal(context.TraceId.ToHexString(), header.Split('-')[1]);
        Assert.Equal(context.SpanId.ToHexString(), header.Split('-')[2]);
    }

    [Fact]
    public void Adversarial_A_The_sampled_flag_reflects_the_context()
    {
        // The byte everyone drops, and dropping it is expensive in a way nothing
        // reports. Hard-code it to 01 and an unsampled trace becomes fully recorded at
        // the first hop, so the sampling rate quietly stops meaning anything.
        // Hard-code it to 00 and every downstream service discards spans the caller
        // wanted, and the trace ends at the boundary with no error anywhere.
        using var ctx = new TelemetryContext();

        var sampled = Ex020_W3CTraceContext.FormatTraceParent(Context(ActivityTraceFlags.Recorded));
        var unsampled = Ex020_W3CTraceContext.FormatTraceParent(Context(ActivityTraceFlags.None));

        Assert.EndsWith("-01", sampled);
        Assert.EndsWith("-00", unsampled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void A_formatted_header_round_trips_back_to_the_same_context(bool recorded)
    {
        using var ctx = new TelemetryContext();
        var original = Context(recorded ? ActivityTraceFlags.Recorded : ActivityTraceFlags.None);

        var parsed = Ex020_W3CTraceContext.ParseTraceParent(
            Ex020_W3CTraceContext.FormatTraceParent(original), "vendor=abc");

        Assert.Equal(original.TraceId, parsed.TraceId);
        Assert.Equal(original.SpanId, parsed.SpanId);
        Assert.Equal(original.TraceFlags, parsed.TraceFlags);
        Assert.Equal("vendor=abc", parsed.TraceState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-header")]
    [InlineData("00-abc-def-01")]
    [InlineData("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7")]
    [InlineData("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01-extra")]
    [InlineData("00-zzf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01")]
    [InlineData("00-00000000000000000000000000000000-00f067aa0ba902b7-01")]
    public void Adversarial_B_A_malformed_header_is_refused_cleanly(string header)
    {
        // This string arrives from outside the process - from a partner, a proxy, a
        // load generator, an attacker. A parser that throws turns a malformed header
        // into a 500 on a request that was otherwise fine; one that half-succeeds
        // starts a trace with an all-zero id that no backend will accept. Refusing and
        // starting fresh is the only behaviour that keeps the request working.
        //
        // The last case is the subtle one: an all-zero trace id is correctly shaped
        // and explicitly invalid per the spec.
        using var ctx = new TelemetryContext();

        var parsed = Ex020_W3CTraceContext.ParseTraceParent(header, traceState: null);

        Assert.Equal(default, parsed);
    }

    [Fact]
    public void A_real_header_from_the_specification_parses_to_its_documented_values()
    {
        // The example straight out of the W3C Trace Context specification. Round-trip
        // tests can agree with themselves while both halves are wrong; this one cannot.
        using var ctx = new TelemetryContext();

        var parsed = Ex020_W3CTraceContext.ParseTraceParent(
            "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01", traceState: null);

        Assert.Equal("4bf92f3577b34da6a3ce929d0e0e4736", parsed.TraceId.ToHexString());
        Assert.Equal("00f067aa0ba902b7", parsed.SpanId.ToHexString());
        Assert.Equal(ActivityTraceFlags.Recorded, parsed.TraceFlags);
    }
}
