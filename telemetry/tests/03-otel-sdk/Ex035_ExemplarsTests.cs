using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex035_ExemplarsTests
{
    private static (string? TraceId, IReadOnlyList<(string TraceId, double Value)> Exemplars) Run(
        ExemplarFilterType filter, bool insideASpan, double milliseconds = 42)
    {
        var exported = new List<Metric>();

        using var tracing = Ex035_Exemplars.BuildTracing();
        using var metrics = Ex035_Exemplars.BuildMetrics(exported, filter);

        string? traceId = null;
        if (insideASpan)
        {
            traceId = Ex035_Exemplars.InsideASpan(() => Ex035_Exemplars.RecordDuration(milliseconds));
        }
        else
        {
            Ex035_Exemplars.RecordDuration(milliseconds);
        }

        metrics.ForceFlush();

        var exemplars = MetricReadout.Of(exported)
            .Where(p => p.Instrument == Ex035_Exemplars.InstrumentName)
            .SelectMany(p => p.Exemplars)
            .ToArray();

        return (traceId, exemplars);
    }

    [Fact]
    public void A_measurement_inside_a_recorded_span_carries_that_spans_trace_id()
    {
        // The bridge between the two halves of everything before this row. Metrics tell
        // you THAT something is slow and cost almost nothing to keep; traces tell you WHY
        // and cost too much to keep all of. Without an exemplar, the graph says the p99
        // doubled at 14:03 and finding one actual slow request means guessing at a search.
        using var ctx = new TelemetryContext();

        var (traceId, exemplars) = Run(ExemplarFilterType.TraceBased, insideASpan: true);

        Assert.NotNull(traceId);
        var exemplar = Assert.Single(exemplars);
        Assert.Equal(traceId, exemplar.TraceId);
    }

    [Fact]
    public void The_exemplar_carries_the_measurements_own_value()
    {
        // The aggregate stays an aggregate; a handful of its inputs remember where they
        // came from - including what they measured, so the link goes to a request that
        // really was this slow.
        using var ctx = new TelemetryContext();

        var (_, exemplars) = Run(ExemplarFilterType.TraceBased, insideASpan: true, milliseconds: 137);

        Assert.Equal(137d, Assert.Single(exemplars).Value);
    }

    [Fact]
    public void Adversarial_A_A_measurement_outside_any_span_gets_no_exemplar()
    {
        // Trace-based means "only from spans that were sampled", and that is exactly
        // right: an exemplar pointing at a trace nobody stored is a link to a 404.
        //
        // The consequence is worth stating because it is invisible until someone says it:
        // the value of your exemplars is bounded by your sampling rate, so the two
        // settings have to be chosen together.
        using var ctx = new TelemetryContext();

        var (traceId, exemplars) = Run(ExemplarFilterType.TraceBased, insideASpan: false);

        Assert.Null(traceId);
        Assert.Empty(exemplars);
    }

    [Fact]
    public void Adversarial_B_With_the_filter_off_nothing_is_attached_even_inside_a_span()
    {
        // The paired half, and what makes the first fact mean something. Exemplars are
        // OFF by default, so a pipeline that produces them has been configured to; a test
        // that only ever checks the happy path cannot tell "the filter worked" from "the
        // SDK does this anyway".
        using var ctx = new TelemetryContext();

        var (traceId, exemplars) = Run(ExemplarFilterType.AlwaysOff, insideASpan: true);

        Assert.NotNull(traceId);
        Assert.Empty(exemplars);
    }

    [Fact]
    public void Adversarial_C_The_recording_code_says_nothing_about_exemplars()
    {
        // The same RecordDuration call produces an exemplar under one filter and none
        // under another. That is the row's real claim: attaching them is the pipeline's
        // job, and the code that measures never knows.
        using var ctx = new TelemetryContext();

        var on = Run(ExemplarFilterType.TraceBased, insideASpan: true);
        var off = Run(ExemplarFilterType.AlwaysOff, insideASpan: true);

        Assert.Single(on.Exemplars);
        Assert.Empty(off.Exemplars);
    }
}
