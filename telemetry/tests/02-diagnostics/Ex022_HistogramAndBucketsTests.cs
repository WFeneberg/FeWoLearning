using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex022_HistogramAndBucketsTests
{
    /// <summary>A healthy service with one catastrophic request.</summary>
    private static readonly double[] Spiky = [10, 10, 10, 10, 1000];

    /// <summary>Uniformly mediocre. Same sum, same count, same mean of 208ms.</summary>
    private static readonly double[] Flat = [208, 208, 208, 208, 208];

    private static MeasurementProbe Record(double[] durations)
    {
        var probe = new MeasurementProbe(Ex022_HistogramAndBuckets.MeterName);
        foreach (var duration in durations)
            Ex022_HistogramAndBuckets.RecordRequest(duration, "/orders/{id}");

        return probe;
    }

    [Fact]
    public void One_request_records_on_all_three_instruments()
    {
        using var probe = Record([42]);

        Assert.Equal(42d, Assert.Single(probe.For(Ex022_HistogramAndBuckets.DurationHistogram)).Value);
        Assert.Equal(42d, Assert.Single(probe.For(Ex022_HistogramAndBuckets.DurationSumCounter)).Value);
        Assert.Equal(1d, Assert.Single(probe.For(Ex022_HistogramAndBuckets.RequestCounter)).Value);
    }

    [Fact]
    public void Every_measurement_carries_the_route_and_the_histogram_declares_milliseconds()
    {
        using var probe = Record([42]);

        Assert.All(probe.Measurements, m => Assert.Equal("/orders/{id}", m.Tag(Ex022_HistogramAndBuckets.RouteTag)));
        Assert.Equal(
            Ex022_HistogramAndBuckets.DurationUnit,
            probe.UnitOf(Ex022_HistogramAndBuckets.DurationHistogram));
    }

    [Fact]
    public void Adversarial_A_The_durations_are_recorded_exactly_as_given()
    {
        // Rounding a duration to a bucket boundary before recording it looks like a
        // helpful optimisation and throws away the only copy of the data. Bucketing is
        // the AGGREGATOR's job: it happens downstream, where the boundaries are
        // configurable, and it cannot be undone once it has been done early.
        using var probe = Record([0.5, 3.25, 999.75]);

        Assert.Equal(
            [0.5, 3.25, 999.75],
            probe.For(Ex022_HistogramAndBuckets.DurationHistogram).Select(m => m.Value));
    }

    /// <summary>
    /// The mean a sum/count pair can produce, and how many samples exceeded a second.
    ///
    /// Each call disposes its probe before returning, which matters: a listener that is
    /// still alive receives the NEXT batch too, and two overlapping probes would each
    /// see both distributions.
    /// </summary>
    private static (double Mean, int OverASecond) Summarise(double[] durations)
    {
        using var probe = Record(durations);

        var sum = probe.For(Ex022_HistogramAndBuckets.DurationSumCounter).Sum(m => m.Value);
        var count = probe.For(Ex022_HistogramAndBuckets.RequestCounter).Sum(m => m.Value);
        var overASecond = probe.For(Ex022_HistogramAndBuckets.DurationHistogram).Count(m => m.Value >= 1000);

        return (sum / count, overASecond);
    }

    [Fact]
    public void Adversarial_B_The_sum_and_count_pair_cannot_tell_the_two_distributions_apart()
    {
        // The whole row, on numbers you can check by hand. [10,10,10,10,1000] and
        // [208,208,208,208,208] have the same sum, the same count and therefore the
        // same mean of 208ms. One is a healthy service with one catastrophic request;
        // the other is uniformly mediocre. An average cannot tell you which - and an
        // average is all a sum/count pair can ever produce.
        var spiky = Summarise(Spiky);
        var flat = Summarise(Flat);

        Assert.Equal(208d, spiky.Mean);
        Assert.Equal(208d, flat.Mean);
    }

    [Fact]
    public void Adversarial_C_The_histogram_can_tell_them_apart()
    {
        // The paired fact, and the reason to pay for a distribution. Every real
        // question - the p99, "how many requests took over a second", "did the slow
        // tail get worse this week" - is answerable here and nowhere in Adversarial_B.
        var spiky = Summarise(Spiky);
        var flat = Summarise(Flat);

        Assert.Equal(1, spiky.OverASecond);
        Assert.Equal(0, flat.OverASecond);
    }
}
