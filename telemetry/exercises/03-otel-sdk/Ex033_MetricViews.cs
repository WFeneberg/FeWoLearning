using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 033 — MetricViews (otel-sdk).
// Goal:   Change what a metric looks like on the way out, without touching the code
//         that records it.
// Drills: AddView - renaming, dropping, tag-key selection, explicit histogram bounds.
// Passes: the requests counter arrives under its new name, and not under its old one;
//         the debug instrument does not arrive at all;
//         user.id is gone from the exported dimensions while http.route survives, so
//                     two records differing only by user COLLAPSE into one series;
//         and the duration histogram carries exactly the bucket boundaries the view
//                     asked for.
//
// A view is the seam between the code that knows what happened and the operator who
// knows what it costs. The instrument stays as the developer wrote it; the view decides
// what leaves the process. That separation is what lets you fix a cardinality
// emergency by configuration at three in the morning instead of by deployment.
//
// The third clause is the emergency in question. Every distinct combination of tag
// values is a separate stored series, forever: a user id on a metric with a hundred
// thousand users is a hundred thousand series per instrument, which is how a metrics
// bill arrives. Dropping the key does not sample or approximate - the measurements are
// all still counted, they are just added into one series instead of a hundred thousand.
//
// The fourth clause matters because the default buckets are a guess about YOUR service.
// The SDK's default boundaries top out in the seconds; if your p99 lives at 40ms, every
// interesting request lands in the same bucket and the histogram can tell you nothing
// you did not already know.
public static class Ex033_MetricViews
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex033";

    /// <summary>What the code calls it.</summary>
    public const string RequestsInstrument = "requests";

    /// <summary>What the operator wants to see.</summary>
    public const string RequestsRenamedTo = "http.server.requests";

    /// <summary>Recorded by the code, wanted by nobody.</summary>
    public const string DebugInstrument = "internal.debug";

    /// <summary>The histogram whose buckets are wrong by default.</summary>
    public const string DurationInstrument = "request.duration";

    /// <summary>A dimension worth keeping: bounded, and every dashboard uses it.</summary>
    public const string RouteTag = "http.route";

    /// <summary>A dimension that must not leave the process: unbounded.</summary>
    public const string UserIdTag = "user.id";

    /// <summary>The boundaries this service actually needs.</summary>
    public static readonly double[] DurationBounds = [5, 10, 25, 50, 100, 250];

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Build a provider reading <see cref="MeterName"/> into <paramref name="exported"/>,
    /// with views that:
    ///
    ///   - rename <see cref="RequestsInstrument"/> to <see cref="RequestsRenamedTo"/>
    ///     and keep only <see cref="RouteTag"/> as a dimension;
    ///   - drop <see cref="DebugInstrument"/> entirely;
    ///   - give <see cref="DurationInstrument"/> the boundaries
    ///     <see cref="DurationBounds"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static MeterProvider Build(ICollection<Metric> exported) =>
        throw new NotImplementedException(
            "TODO: Ex033 - add views that rename, restrict tags, drop, and set explicit bounds");

    /// <summary>
    /// Record one request on all three instruments, tagged with
    /// <paramref name="route"/> and <paramref name="userId"/>: 1 on
    /// <see cref="RequestsInstrument"/>, 1 on <see cref="DebugInstrument"/>, and
    /// <paramref name="milliseconds"/> on <see cref="DurationInstrument"/>.
    ///
    /// The recording code knows nothing about the views. That is the point.
    /// </summary>
    public static void Record(string route, string userId, double milliseconds) =>
        throw new NotImplementedException("TODO: Ex033 - record one request on all three instruments");
}
