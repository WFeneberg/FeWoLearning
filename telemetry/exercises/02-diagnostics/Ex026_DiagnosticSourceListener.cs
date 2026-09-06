using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 026 — DiagnosticSourceListener (diagnostics).
// Goal:   Use the older, richer channel - the one that carries whole OBJECTS rather
//         than tags - and understand what you pay for that.
// Drills: DiagnosticListener, IsEnabled gating, anonymous payloads, Subscribe.
// Passes: with nobody subscribed, neither event is written and countItems is never
//                     called;
//         with a subscriber, Order.Start then Order.Stop arrive, carrying the order id
//                     and the item count;
//         a subscriber whose predicate allows only Order.Stop receives only that one,
//                     and the start payload is never built;
//         and the payloads expose OrderId and ItemCount as PROPERTIES.
//
// The first and third clauses are why IsEnabled exists here and is not optional. An
// ActivitySource costs nothing when unheard because the runtime hands you a null; a
// DiagnosticSource hands you nothing at all, so the payload object is something YOU
// allocate at the call site. Write unconditionally and every request builds an
// anonymous object nobody reads. IsEnabled is checked per event NAME, which is what
// makes it possible for a consumer to take the stop event and skip the start.
//
// The last clause is the trade. There is no schema: the payload is an object and the
// consumer reads it by reflection, matching property names it was told about in
// documentation. That is what lets this channel carry an HttpRequestMessage or a
// DbCommand whole, and it is why renaming a property is a silent break for every
// consumer - nothing fails to compile, the value simply arrives as null forever.
//
// This is the channel ASP.NET Core, HttpClient and EF Core actually publish on, which
// is why it is worth knowing even though Activity and Meter are where new code goes.
public static class Ex026_DiagnosticSourceListener
{
    /// <summary>The name a subscriber uses to find this source.</summary>
    public const string SourceName = "FeWoLearning.Telemetry.Ex026";

    /// <summary>Written before the work.</summary>
    public const string StartEventName = "Order.Start";

    /// <summary>Written after it.</summary>
    public const string StopEventName = "Order.Stop";

    /// <summary>The one source this exercise writes to.</summary>
    public static DiagnosticListener Source { get; } = new(SourceName);

    /// <summary>
    /// Write <see cref="StartEventName"/> with a payload exposing
    /// <c>OrderId</c>, then run the work, then write <see cref="StopEventName"/> with a
    /// payload exposing <c>OrderId</c> and <c>ItemCount</c>.
    ///
    /// Check <see cref="DiagnosticSource.IsEnabled(string)"/> for each event before
    /// building anything - including before calling
    /// <paramref name="countItems"/>, which stands in for work you would not want to do
    /// for a payload nobody reads.
    /// </summary>
    public static void ProcessOrder(string orderId, Func<int> countItems) =>
        throw new NotImplementedException(
            "TODO: Ex026 - write the start and stop events, each gated by IsEnabled for its own name");

    /// <summary>
    /// Subscribe to <see cref="SourceName"/> and nothing else.
    ///
    /// <paramref name="isEnabled"/> decides, per event name, whether the source should
    /// bother writing it at all; <paramref name="onEvent"/> receives the ones that
    /// survive, as (name, payload).
    ///
    /// The caller disposes the result.
    /// </summary>
    public static IDisposable Subscribe(Predicate<string> isEnabled, Action<string, object?> onEvent) =>
        throw new NotImplementedException(
            "TODO: Ex026 - subscribe to this one source, honouring the per-event predicate");
}
