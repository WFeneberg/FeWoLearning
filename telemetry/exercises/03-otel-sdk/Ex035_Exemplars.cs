using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 035 — Exemplars (otel-sdk).
// Goal:   Get from "the p99 got worse" to "here is one of the slow requests" in one
//         click.
// Drills: ExemplarFilterType, exemplars on a metric point, the trace id they carry.
// Passes: with the trace-based filter and a recorded span in scope, the measurement
//                     carries an exemplar whose trace id is that span's;
//         the exemplar's value is the measurement's own value;
//         with no span in scope, the trace-based filter attaches nothing;
//         and with the filter off, nothing is attached even inside a recorded span.
//
// An exemplar is the bridge between the two halves of everything before this row.
// Metrics tell you THAT something is slow and cost almost nothing to keep; traces tell
// you WHY and cost too much to keep all of. The gap between them is the worst part of
// an incident: the graph says the p99 doubled at 14:03, and finding one actual request
// from 14:03 that was slow means guessing at a search.
//
// An exemplar closes it by stapling a trace id onto a sample of the measurements. The
// aggregate stays an aggregate; a handful of its inputs remember where they came from.
//
// The third and fourth clauses are why the filter is a choice rather than a default.
// Trace-based means "only from spans that were sampled", which is exactly right:
// an exemplar pointing at a trace nobody stored is a link to a 404. So the value of
// your exemplars is bounded by your sampling rate, and the two settings have to be
// chosen together - which is the sort of thing that is obvious once stated and
// invisible until then.
public static class Ex035_Exemplars
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex035";

    /// <summary>The source whose spans the exemplars point at.</summary>
    public const string SourceName = "fewolearning.telemetry.ex035.src";

    /// <summary>The instrument the exemplars hang off.</summary>
    public const string InstrumentName = "request.duration";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a tracer provider for <see cref="SourceName"/> that records everything,
    /// so there is a sampled span for an exemplar to point at.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider BuildTracing() =>
        throw new NotImplementedException("TODO: Ex035 - build a tracer provider that records this source");

    /// <summary>
    /// Build a meter provider reading <see cref="MeterName"/> into
    /// <paramref name="exported"/>, with the given exemplar
    /// <paramref name="filter"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static MeterProvider BuildMetrics(ICollection<Metric> exported, ExemplarFilterType filter) =>
        throw new NotImplementedException(
            "TODO: Ex035 - build a manual-collect meter provider with the given exemplar filter");

    /// <summary>
    /// Record <paramref name="milliseconds"/> on a <see cref="double"/> histogram named
    /// <see cref="InstrumentName"/>, with unit "ms". One instrument, created once.
    ///
    /// Nothing here mentions exemplars: attaching them is the filter's job, and the
    /// recording code never knows.
    /// </summary>
    public static void RecordDuration(double milliseconds) =>
        throw new NotImplementedException("TODO: Ex035 - record the duration on the histogram");

    /// <summary>
    /// Run <paramref name="work"/> inside one span from <see cref="Source"/>, and
    /// return that span's trace id as a lowercase hex string - or null if nothing was
    /// listening.
    /// </summary>
    public static string? InsideASpan(Action work) =>
        throw new NotImplementedException("TODO: Ex035 - run the work inside a span and report its trace id");
}
