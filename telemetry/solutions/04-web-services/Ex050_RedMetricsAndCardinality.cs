using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 050 — RedMetricsAndCardinality (web-services).
// Goal:   Build the three numbers every service dashboard is made of, and keep them
//         affordable.
// Drills: Rate, Errors and Duration on one route dimension; an explicit cardinality
//         budget.
// Passes: every request adds 1 to the request counter and one measurement to the duration
//                     histogram;
//         both carry the route and the status class, so a rate AND an error rate are
//                     computable from them;
//         a route the service does not serve collapses to a single sentinel value;
//         and a route it does serve is left exactly as it is.
//
// RED - Rate, Errors, Duration - is not a framework, it is the observation that three
// numbers answer almost every question anyone asks about a service in an incident: how
// much is it doing, how much of that is failing, and how long is it taking. Two
// instruments produce all three, because a counter's rate is a rate and a histogram
// carries its own count.
//
// The third and fourth clauses are the pair that keeps it from bankrupting you, and they
// have to be a pair. Every distinct combination of dimension values is a stored series,
// billed forever: put a raw URL path in there and a service with a million orders has a
// million series per instrument. Collapse EVERYTHING and the metric answers nothing. The
// budget is an allowlist - the routes you actually serve, which is a number you wrote
// down - and one bucket for the rest.
//
// The status is bucketed to its CLASS rather than its exact code for the same reason,
// with a smaller number attached: 2xx/4xx/5xx is three values that answer "is it broken",
// where every distinct code is a few dozen values that answer very slightly more. Both
// are defensible; what is not defensible is not having decided.
//
// Row 033 did the same thing one layer down, with a view, at the point where metrics
// leave the process. Doing it HERE instead means the unbounded value never enters an
// instrument at all - and a view cannot save you from a dimension the exporter never
// sees, which is what makes this the belt to that view's braces.
public static class Ex050_RedMetricsAndCardinality
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex050";

    /// <summary>Rate, and - through the status dimension - errors.</summary>
    public const string RequestCounter = "http.server.requests";

    /// <summary>Duration, in seconds, as the conventions ask for.</summary>
    public const string DurationHistogram = "http.server.request.duration";

    /// <summary>Seconds, in UCUM.</summary>
    public const string DurationUnit = "s";

    /// <summary>The dimension carrying which route was served.</summary>
    public const string RouteTag = "http.route";

    /// <summary>The dimension carrying how it went.</summary>
    public const string StatusClassTag = "http.response.status_class";

    /// <summary>Everything this service actually serves. The budget is this list.</summary>
    public static readonly string[] KnownRoutes = ["/orders/{id}", "/orders", "/health"];

    /// <summary>Where every other route goes, all of them together.</summary>
    public const string OtherRoute = "other";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// The route dimension for <paramref name="route"/>: itself when it is one of
    /// <see cref="KnownRoutes"/>, and <see cref="OtherRoute"/> otherwise.
    /// </summary>
    private static readonly Counter<long> Requests = Meter.CreateCounter<long>(
        RequestCounter, "{request}", "Requests served, by route and status class.");

    private static readonly Histogram<double> Duration = Meter.CreateHistogram<double>(
        DurationHistogram, DurationUnit, "How long serving a request took.");

    public static string BucketRoute(string? route) =>
        route is not null && KnownRoutes.Contains(route, StringComparer.Ordinal)
            ? route
            : OtherRoute;

    /// <summary>
    /// The status dimension for <paramref name="statusCode"/>: "2xx", "3xx", "4xx" or
    /// "5xx", and "other" for anything outside 200-599.
    /// </summary>
    public static string BucketStatus(int statusCode) => statusCode switch
    {
        >= 200 and < 300 => "2xx",
        >= 300 and < 400 => "3xx",
        >= 400 and < 500 => "4xx",
        >= 500 and < 600 => "5xx",
        _ => OtherRoute,
    };

    /// <summary>
    /// Record one served request on both instruments, dimensioned by the BUCKETED route
    /// and status.
    ///
    /// One instrument each, created once.
    /// </summary>
    public static void Record(string? route, int statusCode, double seconds)
    {
        // Bucketed BEFORE the instrument sees them, which is the difference between this
        // and row 033's view: a view cannot save you from a dimension the exporter never
        // receives, and here the unbounded value never enters an instrument at all.
        KeyValuePair<string, object?>[] tags =
        [
            new(RouteTag, BucketRoute(route)),
            new(StatusClassTag, BucketStatus(statusCode)),
        ];

        Requests.Add(1, tags);
        Duration.Record(seconds, tags);
    }
}
