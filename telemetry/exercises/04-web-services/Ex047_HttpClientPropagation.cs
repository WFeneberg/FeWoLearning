using System.Diagnostics;
using System.Net.Http;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 047 — HttpClientPropagation (web-services).
// Goal:   Write the outgoing half of a trace by hand, so the header that crosses the
//         boundary stops being magic.
// Drills: a DelegatingHandler, ActivityKind.Client, injecting the context, extracting it
//         on the other side.
// Passes: an outgoing request carries a traceparent header;
//         the client span is a Client kind and a child of whatever was ambient;
//         the header names the CLIENT span, not the ambient one;
//         the receiving side starts a span whose parent is that client span, on the same
//                     trace, marked remote;
//         and with nothing listening the handler still sends the request and throws
//                     nothing.
//
// The third clause is the one that is wrong in most hand-rolled propagation, and it is
// invisible in a two-service system. Injecting the AMBIENT context sends the caller's own
// parent, so the remote span attaches one level too high: the client span and the server
// span become siblings instead of parent and child, the waterfall loses the network hop
// entirely, and the time spent in transit is silently attributed to the caller.
//
// The last clause is row 015 arriving where it costs the most. StartActivity returns null
// with no listener, so every line of a propagating handler has to work when there is no
// span - because the alternative is an application that throws in production and works in
// every test, or one that quietly stops making outbound calls when telemetry is disabled.
//
// This row is deliberately hand-written rather than using AddHttpClientInstrumentation,
// for a measured reason: that instrumentation listens to a diagnostics handler which the
// real socket handler chain inserts, so a client built over ANY custom handler - which is
// what a test gives you - produces zero spans. See row 041. Writing it out is also the
// only way to see which context gets injected.
public static class Ex047_HttpClientPropagation
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex047";

    /// <summary>The header the trace travels in.</summary>
    public const string TraceParentHeader = "traceparent";

    /// <summary>The name of the span the handler starts.</summary>
    public const string ClientSpanName = "HTTP GET";

    /// <summary>The name of the span the receiving side starts.</summary>
    public const string ServerSpanName = "GET /remote";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex047 - build a provider recording this source");

    /// <summary>
    /// A handler that, around every request it forwards:
    ///
    ///   - starts a <see cref="ClientSpanName"/> span of kind
    ///     <see cref="ActivityKind.Client"/>;
    ///   - writes ITS OWN context into the request as
    ///     <see cref="TraceParentHeader"/>.
    ///
    /// It must forward the request and return the response whether or not anything is
    /// listening.
    /// </summary>
    public static DelegatingHandler CreatePropagatingHandler() =>
        throw new NotImplementedException(
            "TODO: Ex047 - start a client span and inject its own context into the outgoing request");

    /// <summary>
    /// The receiving side: read <paramref name="headers"/>, and start a
    /// <see cref="ServerSpanName"/> span of kind <see cref="ActivityKind.Server"/> as a
    /// child of whatever came in.
    ///
    /// Return the span, stopped.
    /// </summary>
    public static Activity? HandleIncoming(IReadOnlyDictionary<string, string> headers) =>
        throw new NotImplementedException(
            "TODO: Ex047 - continue the incoming trace on the receiving side");
}
