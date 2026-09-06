using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex050_RedMetricsAndCardinalityTests
{
    private static IReadOnlyList<MetricPointSnapshot> Record(
        params (string? Route, int Status, double Seconds)[] requests)
    {
        using var probe = new MetricProbe(Ex050_RedMetricsAndCardinality.MeterName);

        foreach (var (route, status, seconds) in requests)
            Ex050_RedMetricsAndCardinality.Record(route, status, seconds);

        return probe.Collect();
    }

    [Fact]
    public void Rate_and_duration_are_both_recorded_for_one_request()
    {
        var points = Record(("/orders/{id}", 200, 0.012));

        var counter = Assert.Single(points, p => p.Instrument == Ex050_RedMetricsAndCardinality.RequestCounter);
        var duration = Assert.Single(points, p => p.Instrument == Ex050_RedMetricsAndCardinality.DurationHistogram);

        Assert.Equal(1d, counter.Sum);
        Assert.Equal(1, duration.Count);
        Assert.Equal(0.012, duration.Sum, 6);
    }

    [Fact]
    public void Both_instruments_carry_the_route_and_the_status_class()
    {
        // Two instruments produce all three RED numbers, because a counter's rate is a
        // rate and a histogram carries its own count. The status dimension is what turns
        // the first of them into an error rate.
        var points = Record(("/orders/{id}", 500, 0.5));

        Assert.All(points, p =>
        {
            Assert.Equal("/orders/{id}", p.Tag(Ex050_RedMetricsAndCardinality.RouteTag));
            Assert.Equal("5xx", p.Tag(Ex050_RedMetricsAndCardinality.StatusClassTag));
        });
    }

    [Fact]
    public void An_error_rate_is_computable_from_the_counter_alone()
    {
        var points = Record(
            ("/orders/{id}", 200, 0.01),
            ("/orders/{id}", 200, 0.01),
            ("/orders/{id}", 200, 0.01),
            ("/orders/{id}", 503, 0.4));

        var byClass = points
            .Where(p => p.Instrument == Ex050_RedMetricsAndCardinality.RequestCounter)
            .ToDictionary(p => p.Tag(Ex050_RedMetricsAndCardinality.StatusClassTag)!, p => p.Sum);

        Assert.Equal(3d, byClass["2xx"]);
        Assert.Equal(1d, byClass["5xx"]);
    }

    [Fact]
    public void Adversarial_A_Unknown_routes_collapse_into_one_series()
    {
        // Every distinct combination of dimension values is a stored series, billed
        // forever. Put a raw URL path in there and a service with a million orders has a
        // million series per instrument - which is how a metrics bill arrives.
        //
        // A hundred distinct paths, one series.
        var requests = Enumerable.Range(0, 100)
            .Select(i => ((string?)$"/orders/{i}/items/{i}", 200, 0.01))
            .ToArray();

        var points = Record(requests);

        var counter = Assert.Single(points, p => p.Instrument == Ex050_RedMetricsAndCardinality.RequestCounter);
        Assert.Equal(Ex050_RedMetricsAndCardinality.OtherRoute, counter.Tag(Ex050_RedMetricsAndCardinality.RouteTag));
        Assert.Equal(100d, counter.Sum);
    }

    [Fact]
    public void Adversarial_B_A_route_the_service_serves_is_left_alone()
    {
        // The paired half, and it has to be a pair. Collapsing EVERYTHING would satisfy
        // Adversarial_A perfectly and leave the metric unable to answer the one question
        // every dashboard asks. The budget is an allowlist - the routes you actually
        // serve, which is a number you wrote down - and one bucket for the rest.
        var points = Record(
            ("/orders/{id}", 200, 0.01),
            ("/health", 200, 0.001),
            ("/nonsense/9", 404, 0.002));

        var routes = points
            .Where(p => p.Instrument == Ex050_RedMetricsAndCardinality.RequestCounter)
            .Select(p => p.Tag(Ex050_RedMetricsAndCardinality.RouteTag) ?? "<null>")
            .OrderBy(r => r, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["/health", "/orders/{id}", Ex050_RedMetricsAndCardinality.OtherRoute], routes);
    }

    [Theory]
    [InlineData(204, "2xx")]
    [InlineData(301, "3xx")]
    [InlineData(404, "4xx")]
    [InlineData(503, "5xx")]
    [InlineData(101, "other")]
    public void The_status_class_is_the_code_reduced_to_its_hundreds(int code, string expected)
    {
        // Three or four values that answer "is it broken", rather than a few dozen that
        // answer very slightly more. Both are defensible; not having decided is not.
        Assert.Equal(expected, Ex050_RedMetricsAndCardinality.BucketStatus(code));
    }

    [Fact]
    public void The_duration_histogram_declares_seconds()
    {
        // The conventions ask for seconds, and a dashboard that assumes milliseconds
        // against a service reporting seconds is off by a factor of a thousand in the
        // direction that looks fine.
        using var probe = new MetricProbe(Ex050_RedMetricsAndCardinality.MeterName);
        Ex050_RedMetricsAndCardinality.Record("/orders/{id}", 200, 0.012);

        var duration = Assert.Single(probe.CollectFor(Ex050_RedMetricsAndCardinality.DurationHistogram));
        Assert.Equal(Ex050_RedMetricsAndCardinality.DurationUnit, duration.Unit);
    }
}
