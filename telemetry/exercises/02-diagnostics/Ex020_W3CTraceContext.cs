using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 020 — W3CTraceContext (diagnostics).
// Goal:   Write and read the header that makes a trace survive a process boundary.
// Drills: the traceparent wire format, tracestate, ActivityContext round-tripping.
// Passes: FormatTraceParent produces exactly "00-<32 hex>-<16 hex>-<2 hex>";
//         the sampled flag renders as "01" and an unsampled one as "00";
//         Parse round-trips a formatted header back to the same trace id, span id and
//                     sampled flag, and carries tracestate through untouched;
//         and a malformed header yields default - no exception, no half-built context.
//
// The second clause is the byte everyone drops, and dropping it is expensive in a way
// nothing reports. The flags byte is how a caller tells everyone downstream "this trace
// is being recorded". Hard-code it to 01 and an unsampled trace turns into a fully
// recorded one at the first hop, and your sampling rate quietly stops meaning anything.
// Hard-code it to 00 and every downstream service drops spans the caller wanted, and
// the trace ends at the boundary with no error anywhere.
//
// The last clause is the one that shows up as an outage. This header arrives from
// outside your process - from a partner, a proxy, a load generator, an attacker - and
// it is a STRING. A parser that throws on a bad one turns a malformed header into a 500
// on a request that was otherwise fine; a parser that half-succeeds starts a trace with
// an all-zero span id that no backend will accept. Refusing cleanly and starting a
// fresh trace is the only behaviour that keeps the request working.
public static class Ex020_W3CTraceContext
{
    /// <summary>The only traceparent version this exercise handles.</summary>
    public const string Version = "00";

    /// <summary>
    /// Render <paramref name="context"/> as a W3C <c>traceparent</c> header value:
    /// the version, the 32-hex trace id, the 16-hex span id and the two-hex flags
    /// byte, joined by hyphens.
    ///
    /// The flags byte is "01" when <see cref="ActivityTraceFlags.Recorded"/> is set
    /// and "00" otherwise.
    /// </summary>
    public static string FormatTraceParent(ActivityContext context) =>
        throw new NotImplementedException("TODO: Ex020 - render the context as a traceparent header value");

    /// <summary>
    /// Parse a <c>traceparent</c> header value, carrying
    /// <paramref name="traceState"/> through onto the result.
    ///
    /// Return <c>default</c> for anything malformed. Never throw: this string came
    /// from outside the process.
    /// </summary>
    public static ActivityContext ParseTraceParent(string traceParent, string? traceState) =>
        throw new NotImplementedException("TODO: Ex020 - parse the header, refusing malformed input cleanly");
}
