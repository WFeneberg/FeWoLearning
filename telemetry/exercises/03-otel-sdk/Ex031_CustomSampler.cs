using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 031 — CustomSampler (otel-sdk).
// Goal:   Decide per span, on the attributes you have at the moment it starts - which
//         is fewer than you think.
// Drills: Sampler, SamplingParameters, Drop vs RecordOnly vs RecordAndSample.
// Passes: a business route is recorded, requests all data, and is exported;
//         the health route is DROPPED - a real activity, no data requested, no export;
//         the cache route is RECORD ONLY - a real activity that DOES request all data,
//                     is not sampled, and is still never exported;
//         and a route tag set AFTER the span started does not change the decision.
//
// The third clause is the one worth the exercise. RecordOnly is not "half sampled", it
// is "build the data, do not propagate the decision, do not export". The span is fully
// populated, so a processor can read it and turn it into a metric or a log; but
// Recorded is false, so the exporter skips it and every downstream service sees an
// unsampled traceparent. It exists for exactly that: local analysis of spans you have
// no intention of storing.
//
// The fourth clause is the limitation everyone hits once. A sampler runs BEFORE the
// span exists - that is the point, since its answer determines whether the span gets
// built at all - so it sees only the attributes passed to StartActivity, never the ones
// set afterwards. Deciding on something you learn later (a status code, a duration, a
// user's plan) is not possible here; that is what tail sampling in a collector is for,
// and it is why row 056 exists.
public sealed class Ex031_CustomSampler : Sampler
{
    /// <summary>The attribute the decision is made on.</summary>
    public const string RouteTag = "http.route";

    /// <summary>Dropped: pure noise, and there is a lot of it.</summary>
    public const string HealthRoute = "/health";

    /// <summary>Recorded but not sampled: useful locally, not worth storing.</summary>
    public const string CacheRoute = "/cache";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "request";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new("fewolearning.telemetry.ex031");

    /// <summary>
    /// Decide from <see cref="SamplingParameters.Tags"/>:
    ///
    ///   <see cref="HealthRoute"/> - <see cref="SamplingDecision.Drop"/>;
    ///   <see cref="CacheRoute"/>  - <see cref="SamplingDecision.RecordOnly"/>;
    ///   anything else, or no route at all - <see cref="SamplingDecision.RecordAndSample"/>.
    /// </summary>
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters) =>
        throw new NotImplementedException(
            "TODO: Ex031 - decide from the initial route tag, using all three decisions");

    /// <summary>
    /// Build a provider listening to <see cref="Source"/>, sampling with an instance of
    /// this class, exporting into <paramref name="exported"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex031 - build a provider using this sampler");

    /// <summary>
    /// Start one <see cref="WorkSpanName"/> span whose route tag is
    /// <paramref name="routeAtStart"/> AT START - which is the only form the sampler
    /// will ever see.
    ///
    /// When <paramref name="routeAfterStart"/> is given, overwrite the tag once the span
    /// is running, to show that it changes nothing.
    /// </summary>
    public static Activity? DoWork(string routeAtStart, string? routeAfterStart = null) =>
        throw new NotImplementedException(
            "TODO: Ex031 - start the span with the route as an INITIAL tag, then optionally overwrite it");
}
