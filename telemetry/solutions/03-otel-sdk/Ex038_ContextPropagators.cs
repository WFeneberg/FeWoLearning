using OpenTelemetry.Context.Propagation;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 038 — ContextPropagators (otel-sdk).
// Goal:   Move the trace across a boundary the SDK knows nothing about, by hand.
// Drills: TextMapPropagator, Inject and Extract, the composite the SDK defaults to.
// Passes: Inject writes a well-formed traceparent into the carrier;
//         Extract round-trips it back to the same trace id, span id and sampled flag,
//                     marked as REMOTE;
//         the same pair carries BAGGAGE as well, in its own header;
//         and extracting from a carrier with no headers yields default - no exception,
//                     no invented context.
//
// Row 020 wrote the traceparent header by hand to see what is in it. This row uses the
// SDK's propagator, and the difference is not convenience: the propagator is the seam
// every instrumentation library plugs into, so a custom transport that uses it inherits
// baggage, tracestate and any future header for free, while one that formats
// traceparent itself inherits exactly traceparent forever.
//
// The third clause is why the DEFAULT is a composite rather than the trace propagator
// alone. Two propagators, two independent headers, one call - and a hand-rolled
// injector that only writes traceparent drops every piece of baggage at that boundary
// silently. Nothing downstream can tell the difference between "no baggage was set" and
// "the hop threw it away".
//
// The fourth clause is the same lesson as row 020's malformed-header fact, one level up:
// this carrier came from outside. A missing header is normal - it is what the first hop
// of every trace looks like - and the correct answer is an empty context, from which the
// SDK starts a fresh trace.
public static class Ex038_ContextPropagators
{
    /// <summary>The W3C trace header.</summary>
    public const string TraceParentHeader = "traceparent";

    /// <summary>The W3C baggage header.</summary>
    public const string BaggageHeader = "baggage";

    /// <summary>
    /// The propagator this exercise uses: trace context AND baggage, composed - which
    /// is what the SDK installs by default.
    /// </summary>
    // Two propagators, two independent headers, one call. This is what the SDK installs
    // by default, and a hand-rolled injector that only writes traceparent silently drops
    // every piece of baggage at that boundary.
    private static readonly CompositeTextMapPropagator Composite =
        new([new TraceContextPropagator(), new BaggagePropagator()]);

    public static TextMapPropagator Propagator => Composite;

    /// <summary>
    /// Write <paramref name="context"/> into a fresh carrier and return it. The carrier
    /// stands in for whatever headers your transport has.
    /// </summary>
    public static IDictionary<string, string> Inject(PropagationContext context)
    {
        var carrier = new Dictionary<string, string>();

        Propagator.Inject(context, carrier, (target, key, value) => target[key] = value);

        return carrier;
    }

    /// <summary>
    /// Read a context back out of <paramref name="carrier"/>.
    ///
    /// Return <c>default</c> when it holds nothing to read. Never throw: this came from
    /// outside the process.
    /// </summary>
    public static PropagationContext Extract(IReadOnlyDictionary<string, string> carrier) =>
        // A missing header is not an error - it is what the first hop of every trace
        // looks like. The propagator returns the default context and the SDK starts a
        // fresh trace from it.
        Propagator.Extract(
            default,
            carrier,
            (source, key) => source.TryGetValue(key, out var value) ? [value] : []);
}
