using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 039 — BaggagePropagation (otel-sdk).
// Goal:   Carry a value across a process boundary, and see that arriving is not the
//         same as being recorded.
// Drills: Baggage.SetBaggage, injecting and extracting it, copying it onto a span.
// Passes: the caller's carrier holds a baggage header naming the tenant;
//         after extraction the callee can read that tenant out of Baggage;
//         a span started on the callee side carries NO tenant attribute unless the code
//                     puts one there;
//         and when it does, the attribute is present with the same value.
//
// The third and fourth clauses are one finding stated twice, and it is the thing people
// get wrong about baggage. It is CONTEXT, not data. It travels, it is available to every
// frame under it and to every service downstream - and no backend indexes it, no
// dashboard can filter on it, and no span carries it. A value that is in baggage and
// nowhere else is invisible to everything except code that deliberately reads it.
//
// So the pattern is: propagate in baggage, record on the span at the point where it
// matters. That copy is a decision, not an oversight, and it is where you get to choose
// which services pay for the extra attribute - and, more sharply, which of them are
// allowed to write a tenant id into permanent storage at all.
//
// Which is also the warning. Baggage is copied onto EVERY outbound request for the rest
// of the trace, including to services you do not own. A tenant id is a reasonable thing
// to send that far. A user's email is not, and there is no per-hop filter to stop it.
//
// (`MicroServices/` row 068 measured the same asymmetry from the Aspire side: traceparent
// propagates on its own, baggage does not usefully do anything until something reads it.)
//
// This row deliberately BUILDS ON row 038 and calls its Inject/Extract rather than
// re-deriving them - which also means its facts only hold once 038 is right. Do them in
// order. (Measured while probing: with 038 wrong as well, four of these five facts fail
// instead of the one this row is about.)
public static class Ex039_BaggagePropagation
{
    /// <summary>The baggage key the tenant travels under.</summary>
    public const string TenantBaggageKey = "tenant.id";

    /// <summary>The span attribute it is copied to, when it is.</summary>
    public const string TenantAttribute = "tenant.id";

    /// <summary>The name of the span the callee starts.</summary>
    public const string HandlerSpanName = "handle";

    /// <summary>The source the callee's span comes from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex039";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a tracer provider recording <see cref="SourceName"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider BuildTracing(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex039 - build a provider recording this source");

    /// <summary>
    /// The caller's side: put <paramref name="tenantId"/> into baggage under
    /// <see cref="TenantBaggageKey"/> and inject the current context into a fresh
    /// carrier, which is returned.
    ///
    /// Leave no baggage behind for the rest of this thread.
    /// </summary>
    public static IDictionary<string, string> CallerSide(string tenantId) =>
        throw new NotImplementedException(
            "TODO: Ex039 - set the tenant in baggage, inject it into a carrier, and restore the baggage after");

    /// <summary>
    /// The callee's side: extract <paramref name="carrier"/>, make its baggage current,
    /// start a <see cref="HandlerSpanName"/> span, and report the tenant it can see.
    ///
    /// Copy that tenant onto the span as <see cref="TenantAttribute"/> ONLY when
    /// <paramref name="copyOntoSpan"/> says so - because that copy is a decision.
    /// </summary>
    public static (Activity? Span, string? TenantFromBaggage) CalleeSide(
        IReadOnlyDictionary<string, string> carrier, bool copyOntoSpan) =>
        throw new NotImplementedException(
            "TODO: Ex039 - restore the baggage, start a span, and copy the tenant onto it only if asked");
}
