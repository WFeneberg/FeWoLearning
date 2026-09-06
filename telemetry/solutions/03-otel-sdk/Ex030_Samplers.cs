using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 030 — Samplers (otel-sdk).
// Goal:   Decide what to keep, and understand what "dropped" actually does to a span.
// Drills: AlwaysOn, AlwaysOff, TraceIdRatioBased, ParentBased; recorded vs merely
//         created.
// Passes: with AlwaysOn, work produces a recorded span and one export;
//         with AlwaysOff, work still produces a NON-NULL activity - unrecorded, with no
//                     data requested, and never exported;
//         a ratio of 0.0 behaves as off and 1.0 as on;
//         ParentBased follows a REMOTE parent's sampled flag in both directions,
//                     overriding its own root behaviour.
//
// The second clause is row 015's finding one level up, and it is the whole reason
// sampling can work at all. A dropped span is not absent: it exists, it carries a trace
// id and a span id, and it propagates - so a downstream service still knows which trace
// it belongs to and can make the same decision. What it does not do is record. That is
// why `if (activity.IsAllDataRequested)` matters on the hot path: at a 1% sample rate,
// 99% of your spans are this shape, and any expensive tagging you do unguarded is work
// thrown away 99 times out of 100.
//
// The fourth clause is what keeps a trace whole. If every service sampled
// independently at 10%, a five-hop trace would survive end to end one time in a
// hundred thousand, and the traces you did keep would be full of holes. ParentBased
// says: whoever started this trace already decided - honour it. The head makes the
// choice, everyone downstream obeys, and you get whole traces at the rate you asked
// for instead of fragments at a rate nobody can compute.
public static class Ex030_Samplers
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex030";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "work";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider listening to <see cref="SourceName"/>, sampling with
    /// <paramref name="sampler"/> and exporting into <paramref name="exported"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported, Sampler sampler) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .SetSampler(sampler)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Start and stop one <see cref="WorkSpanName"/> activity and return it.
    ///
    /// When <paramref name="parent"/> is not <c>default</c>, start it as a child of
    /// that context - which is how an incoming traceparent reaches the sampler.
    /// </summary>
    public static Activity? DoWork(ActivityContext parent = default)
    {
        // The parentContext overload is how an incoming traceparent reaches the
        // sampler: ParentBased reads its flags to decide, so passing default here would
        // silently make every span a root and the parent-based facts meaningless.
        using var activity = Source.StartActivity(WorkSpanName, ActivityKind.Internal, parent);

        return activity;
    }

    /// <summary>
    /// A remote parent context with the sampled flag set or clear - what a
    /// <c>traceparent</c> header amounts to once parsed.
    /// </summary>
    public static ActivityContext RemoteParent(bool sampled) =>
        // isRemote: true matters. ParentBased treats remote and local parents through
        // different branches, so a context built without it exercises the wrong one.
        new(
            ActivityTraceId.CreateRandom(),
            ActivitySpanId.CreateRandom(),
            sampled ? ActivityTraceFlags.Recorded : ActivityTraceFlags.None,
            traceState: null,
            isRemote: true);
}
