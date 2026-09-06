namespace FeWoLearning.Architecture.Exercises.Evolution.Ex080;

/// <summary>
/// One unit of work in a trace. TraceId ties the whole request together across every
/// process it touches; ParentSpanId is what makes it a tree rather than a list.
/// </summary>
public sealed record Span(string TraceId, string SpanId, string? ParentSpanId, string Name, bool Sampled);

// Exercise 080 — ObservabilitySpans (reference solution).
public sealed class Tracer(Func<double> random, double sampleRate, Func<string> newId)
{
    public Span StartRoot(string name) =>
        // The decision is made here and ONLY here. Deciding per span produces traces with
        // holes - half the spans of one request kept, half dropped - which is strictly
        // worse than dropping the request entirely, because the gap looks like the work
        // never happened.
        new(newId(), newId(), ParentSpanId: null, name, Sampled: random() < sampleRate);

    public Span StartChild(Span parent, string name) =>
        // Same trace, new span, parent recorded, and the parent's decision inherited
        // rather than reconsidered.
        new(parent.TraceId, newId(), parent.SpanId, name, parent.Sampled);

    public string Serialize(Span span) =>
        $"{span.TraceId}-{span.SpanId}-{(span.Sampled ? "01" : "00")}";

    public Span Continue(string? traceparent, string name)
    {
        // A bad header from somebody else's client must never be able to fail the
        // request. The worst it may cost is one broken trace - and starting a fresh one
        // at least keeps this process's work visible.
        if (string.IsNullOrWhiteSpace(traceparent))
            return StartRoot(name);

        var parts = traceparent.Split('-');

        if (parts.Length != 3 || parts[0].Length == 0 || parts[1].Length == 0 || (parts[2] != "01" && parts[2] != "00"))
            return StartRoot(name);

        // The sampling flag comes from the header, not from another roll of the dice: the
        // decision was made upstream and this process is downstream of it.
        return new Span(parts[0], newId(), parts[1], name, parts[2] == "01");
    }
}
