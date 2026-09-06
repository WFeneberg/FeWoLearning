using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex033_MetricViewsTests
{
    /// <summary>
    /// Records the given requests and returns a snapshot of one collection. Snapshotted
    /// rather than kept as Metric objects, because the exporter reuses those.
    /// </summary>
    private static IReadOnlyList<MetricPointSnapshot> Collect(
        params (string Route, string UserId, double Ms)[] requests)
    {
        var exported = new List<Metric>();

        using var provider = Ex033_MetricViews.Build(exported);
        foreach (var (route, userId, ms) in requests) Ex033_MetricViews.Record(route, userId, ms);
        provider.ForceFlush();

        return MetricReadout.Of(exported);
    }

    [Fact]
    public void The_counter_arrives_under_its_new_name_and_not_its_old_one()
    {
        var points = Collect(("/orders", "u-1", 12));

        Assert.Contains(points, p => p.Instrument == Ex033_MetricViews.RequestsRenamedTo);
        Assert.DoesNotContain(points, p => p.Instrument == Ex033_MetricViews.RequestsInstrument);
    }

    [Fact]
    public void The_dropped_instrument_does_not_arrive_at_all()
    {
        var points = Collect(("/orders", "u-1", 12));

        Assert.DoesNotContain(points, p => p.Instrument == Ex033_MetricViews.DebugInstrument);
    }

    [Fact]
    public void Adversarial_A_The_unbounded_dimension_is_gone_and_the_bounded_one_survives()
    {
        var points = Collect(("/orders", "u-1", 12));

        var counter = Assert.Single(points, p => p.Instrument == Ex033_MetricViews.RequestsRenamedTo);
        Assert.Equal("/orders", counter.Tag(Ex033_MetricViews.RouteTag));
        Assert.Null(counter.Tag(Ex033_MetricViews.UserIdTag));
    }

    [Fact]
    public void Adversarial_B_Two_records_differing_only_by_user_collapse_into_one_series()
    {
        // The emergency this exists for, and the half that Adversarial_A cannot see:
        // dropping the KEY is only useful if the measurements then MERGE. Every distinct
        // combination of tag values is a separate stored series, forever - a user id on
        // a metric with a hundred thousand users is a hundred thousand series per
        // instrument, which is how a metrics bill arrives.
        //
        // Dropping the key does not sample or approximate. All three measurements are
        // still counted; they are added into one series instead of three.
        var points = Collect(
            ("/orders", "u-1", 12),
            ("/orders", "u-2", 15),
            ("/orders", "u-3", 18));

        var counter = Assert.Single(points, p => p.Instrument == Ex033_MetricViews.RequestsRenamedTo);
        Assert.Equal(3d, counter.Sum);
    }

    [Fact]
    public void Adversarial_C_A_dimension_that_was_kept_still_separates_its_series()
    {
        // The paired use fact. A view that dropped ALL dimensions would satisfy
        // Adversarial_A and Adversarial_B perfectly and leave the metric unable to answer
        // the one question every dashboard asks.
        var points = Collect(
            ("/orders", "u-1", 12),
            ("/orders", "u-2", 15),
            ("/invoices", "u-3", 18));

        var byRoute = points
            .Where(p => p.Instrument == Ex033_MetricViews.RequestsRenamedTo)
            .ToDictionary(p => p.Tag(Ex033_MetricViews.RouteTag)!, p => p.Sum);

        Assert.Equal(2d, byRoute["/orders"]);
        Assert.Equal(1d, byRoute["/invoices"]);
    }

    [Fact]
    public void Adversarial_D_The_histogram_carries_the_boundaries_the_view_asked_for()
    {
        // The default boundaries are a guess about YOUR service, and they top out in the
        // seconds. If your p99 lives at 40ms, every interesting request lands in the same
        // bucket and the histogram tells you nothing you did not already know.
        var points = Collect(("/orders", "u-1", 12));

        var histogram = Assert.Single(points, p => p.Instrument == Ex033_MetricViews.DurationInstrument);
        Assert.Equal(Ex033_MetricViews.DurationBounds, histogram.ExplicitBounds);
    }
}
