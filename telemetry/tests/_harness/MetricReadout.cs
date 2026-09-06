using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// One metric point, copied out of the SDK at the moment of collection.
/// </summary>
/// <param name="Instrument">The instrument's name, after any view has renamed it.</param>
/// <param name="Tags">The dimensions that survived to this point.</param>
/// <param name="Sum">The sum, or a gauge's last value, whichever the type carries.</param>
/// <param name="Count">The number of measurements, for histograms; 0 otherwise.</param>
/// <param name="ExplicitBounds">The histogram's finite bucket boundaries, or empty.</param>
/// <param name="Exemplars">Trace ids and values of any exemplars attached.</param>
public sealed record MetricPointSnapshot(
    string Instrument,
    IReadOnlyList<KeyValuePair<string, object?>> Tags,
    double Sum,
    long Count,
    IReadOnlyList<double> ExplicitBounds,
    IReadOnlyList<(string TraceId, double Value)> Exemplars)
{
    /// <summary>The value of one dimension, or null when this point has no such tag.</summary>
    public string? Tag(string key) => Tags.FirstOrDefault(t => t.Key == key).Value?.ToString();
}

/// <summary>
/// Copies metric points out of the SDK's own objects, and it is not a convenience - it
/// is the only correct way to read them.
///
/// Measured 2026-09-06: the in-memory metric exporter appends one <see cref="Metric"/>
/// per collection but hands back THE SAME OBJECT each time. So the exported list grows
/// while every entry for an instrument is one instance, and a value read after a later
/// collection is that later collection's value. A Delta/Cumulative test that holds on to
/// Metric objects agrees with itself while measuring nothing.
///
/// Call <see cref="Of"/> immediately after each collection.
/// </summary>
public static class MetricReadout
{
    /// <summary>
    /// Every metric point currently held, deduplicated by reference so a list that has
    /// accumulated several collections yields one entry per instrument.
    /// </summary>
    public static IReadOnlyList<MetricPointSnapshot> Of(IEnumerable<Metric> exported)
    {
        var distinct = new List<Metric>();
        foreach (var metric in exported)
        {
            if (!distinct.Any(m => ReferenceEquals(m, metric))) distinct.Add(metric);
        }

        return distinct.SelectMany(PointsOf).ToArray();
    }

    private static List<MetricPointSnapshot> PointsOf(Metric metric)
    {
        var points = new List<MetricPointSnapshot>();

        foreach (ref readonly var readOnlyPoint in metric.GetMetricPoints())
        {
            // A writable copy: GetHistogramBuckets is not declared readonly, so calling
            // it on the `ref readonly` loop variable fails with CS1510. MetricPoint holds
            // references to the underlying aggregation, so the copy reads the same data.
            var point = readOnlyPoint;

            var tags = new List<KeyValuePair<string, object?>>();
            foreach (var tag in point.Tags) tags.Add(tag);

            var sum = 0d;
            var count = 0L;
            var bounds = new List<double>();

            if (metric.MetricType.IsHistogram())
            {
                sum = point.GetHistogramSum();
                count = point.GetHistogramCount();

                // By value, not by ref: the HistogramBuckets enumerator yields
                // HistogramBucket by value, so a "ref readonly" loop variable is CS1510.
                // The final +infinity bucket is dropped - it is implied, and no view ever
                // asks for it.
                foreach (var bucket in point.GetHistogramBuckets())
                {
                    if (!double.IsPositiveInfinity(bucket.ExplicitBound)) bounds.Add(bucket.ExplicitBound);
                }
            }
            else if (metric.MetricType.IsGauge())
            {
                sum = metric.MetricType.IsLong()
                    ? point.GetGaugeLastValueLong()
                    : point.GetGaugeLastValueDouble();
            }
            else
            {
                // Calling the wrong accessor throws, so the type has to be consulted.
                sum = metric.MetricType.IsLong() ? point.GetSumLong() : point.GetSumDouble();
            }

            var exemplars = new List<(string TraceId, double Value)>();
            if (point.TryGetExemplars(out var collection))
            {
                foreach (ref readonly var exemplar in collection)
                {
                    exemplars.Add((exemplar.TraceId.ToHexString(), exemplar.DoubleValue));
                }
            }

            points.Add(new MetricPointSnapshot(metric.Name, tags, sum, count, bounds, exemplars));
        }

        return points;
    }
}
