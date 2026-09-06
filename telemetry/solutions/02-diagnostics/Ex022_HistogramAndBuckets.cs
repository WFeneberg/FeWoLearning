using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 022 — HistogramAndBuckets (diagnostics).
// Goal:   See, on real numbers, why an average is not a latency measurement.
// Drills: Histogram<T>, units, recording raw values, what a sum-and-count pair loses.
// Passes: each request records ONE histogram measurement carrying the raw duration and
//                     the route tag, on an instrument named "request.duration" with
//                     unit "ms";
//         each request also adds to the sum counter and the count counter, so the two
//                     approaches can be compared side by side;
//         the histogram values are the durations EXACTLY as given - not rounded, not
//                     bucketed, not clamped;
//         and two very different distributions with the same mean are identical in the
//                     sum/count pair and different in the histogram.
//
// The last clause is the whole row, and the test does it with numbers you can check by
// hand: [10, 10, 10, 10, 1000] and [208, 208, 208, 208, 208] have the same sum, the
// same count and therefore the same mean of 208ms. One of them is a healthy service
// with one catastrophic request; the other is uniformly mediocre. An average cannot
// tell you which, and an average is what a sum/count pair can produce. Every real
// question - the p99, "how many requests took over a second", "did the slow tail get
// worse" - needs the distribution.
//
// The third clause is the mistake that quietly destroys it. Rounding a duration to a
// bucket boundary before recording it looks like a helpful optimisation and throws away
// the only copy of the data: bucketing is the AGGREGATOR's job, it happens downstream
// where the boundaries are configurable, and it cannot be undone once you have done it
// early.
public static class Ex022_HistogramAndBuckets
{
    /// <summary>The name this exercise's meter is registered under.</summary>
    public const string MeterName = "fewolearning.telemetry.ex022";

    /// <summary>The distribution instrument.</summary>
    public const string DurationHistogram = "request.duration";

    /// <summary>The naive alternative: total milliseconds served.</summary>
    public const string DurationSumCounter = "request.duration.sum";

    /// <summary>The naive alternative: how many requests that total covers.</summary>
    public const string RequestCounter = "request.count";

    /// <summary>Milliseconds, in UCUM.</summary>
    public const string DurationUnit = "ms";

    /// <summary>The dimension carrying the route template.</summary>
    public const string RouteTag = "http.route";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Record one finished request, BOTH ways.
    ///
    ///   - one measurement of <paramref name="milliseconds"/> on a
    ///     <see cref="Histogram{T}"/> of <see cref="double"/> named
    ///     <see cref="DurationHistogram"/>, unit <see cref="DurationUnit"/>;
    ///   - <paramref name="milliseconds"/> added to a <see cref="double"/> counter
    ///     named <see cref="DurationSumCounter"/>;
    ///   - 1 added to a <see cref="long"/> counter named <see cref="RequestCounter"/>.
    ///
    /// All three carry <paramref name="route"/> as <see cref="RouteTag"/>. Record the
    /// duration exactly as given.
    /// </summary>
    private static readonly Histogram<double> Duration =
        Meter.CreateHistogram<double>(DurationHistogram, DurationUnit);

    private static readonly Counter<double> DurationSum =
        Meter.CreateCounter<double>(DurationSumCounter, DurationUnit);

    private static readonly Counter<long> Requests =
        Meter.CreateCounter<long>(RequestCounter, "{request}");

    public static void RecordRequest(double milliseconds, string route)
    {
        var tag = new KeyValuePair<string, object?>(RouteTag, route);

        // The raw value, untouched. Bucketing is the aggregator's job - it happens
        // downstream where the boundaries are configurable, and rounding here throws
        // away the only copy.
        Duration.Record(milliseconds, tag);

        // The naive alternative, recorded alongside so the two can be compared. It is
        // not wrong, it is just incapable of answering anything but the mean.
        DurationSum.Add(milliseconds, tag);
        Requests.Add(1, tag);
    }
}
