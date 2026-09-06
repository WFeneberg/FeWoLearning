namespace FeWoLearning.Architecture.Exercises.Evolution.Ex080;

/// <summary>
/// One unit of work in a trace. TraceId ties the whole request together across every
/// process it touches; ParentSpanId is what makes it a tree rather than a list.
/// </summary>
public sealed record Span(string TraceId, string SpanId, string? ParentSpanId, string Name, bool Sampled);

// Exercise 080 — ObservabilitySpans (evolution).
// Goal:   Follow one request across processes, and decide once whether to keep it.
// Drills: trace and span identity, parent/child, context propagation, head-based sampling.
// Passes: root      - a new trace id, a new span id, no parent.
//         child     - the SAME trace id, a NEW span id, the parent's span id as its parent.
//         THE ONE    - the sampling decision is made ONCE, at the root, and INHERITED by
//                     every child and across every boundary. Deciding per span produces
//                     traces with holes - half the spans of one request kept, half
//                     dropped - which is strictly worse than dropping the request
//                     entirely, because the gap looks like the work never happened.
//         propagate - Serialize/Continue carry the trace id, the parent span id and the
//                     sampling flag to another process.
//         THE SECOND - a MALFORMED incoming header starts a new trace instead of throwing.
//                     A bad header from somebody else's client must never be able to fail
//                     the request; the worst it may cost is one broken trace.
//         sampling  - the rate is honoured at the root, using the injected random source.
//
// Head-based sampling - deciding at the first span and carrying the answer - is the
// reason the flag travels in the header at all. The alternative, letting each service
// decide, is cheap to implement and produces exactly the traces nobody can use: the ones
// where the slow call in the middle is the span that was dropped.
//
// The format here is a simplified traceparent: "traceId-spanId-flags", with flags "01"
// sampled and "00" not.
public sealed class Tracer(Func<double> random, double sampleRate, Func<string> newId)
{
    public Span StartRoot(string name) =>
        throw new NotImplementedException(
            "TODO: Ex080 - a fresh trace id and span id, no parent, and the ONE sampling decision for this trace");

    public Span StartChild(Span parent, string name) =>
        throw new NotImplementedException(
            "TODO: Ex080 - same trace id, new span id, the parent's span id, and the parent's sampling decision");

    /// <summary>"traceId-spanId-flags", for the wire.</summary>
    public string Serialize(Span span) =>
        throw new NotImplementedException("TODO: Ex080 - traceId-spanId-flags, with 01 for sampled and 00 for not");

    /// <summary>
    /// Continue a trace from an incoming header, as a child of the span it names. A header
    /// that cannot be parsed starts a new trace instead.
    /// </summary>
    public Span Continue(string? traceparent, string name) =>
        throw new NotImplementedException(
            "TODO: Ex080 - parse the header into a child span carrying the same trace and sampling flag, and fall back to a new root when it is unusable");
}
