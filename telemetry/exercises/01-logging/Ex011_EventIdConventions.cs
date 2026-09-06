using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 011 — EventIdConventions (logging).
// Goal:   Give every event a stable identity that outlives its wording, and keep the
//         identities in one catalog instead of scattered through the call sites.
// Drills: EventId, the id/name pair, one declaration site per event.
// Passes: All lists exactly three events - 1001 OrderAccepted, 1002 OrderRejected,
//                     1003 PaymentRetried;
//         each Log method writes its own id, its own name and its own level;
//         every id that reaches a record is one of the ids in All;
//         and the same event logged with different data keeps one id and one constant
//                     template, with a name that is never empty.
//
// The third clause is the one a catalog exists for. An EventId invented inline at a
// call site looks identical in the log and drifts the moment somebody copies the line
// into a neighbouring method - and then two unrelated events share a number, or one
// event answers to two, and every dashboard and alert built on either quietly starts
// lying. The catalog is the single place that can be reviewed.
//
// The last clause covers the two half-measures. `new EventId(1001)` with no name gives
// you a number nobody can read in a query builder; and an id derived from the message
// changes whenever somebody fixes a typo in the wording, which is exactly the thing an
// id is supposed to survive.
//
// What this row deliberately does NOT grade: that the ids are declared as named
// members rather than repeated literals. Nothing observable distinguishes the two -
// the honest check is that every emitted id appears in All, which is the fact above.
public static class Ex011_EventIdConventions
{
    /// <summary>
    /// Every event this component can write, in id order. Real components expose this
    /// so documentation, dashboards and alert rules can be generated rather than
    /// transcribed.
    /// </summary>
    public static IReadOnlyList<EventId> All =>
        throw new NotImplementedException("TODO: Ex011 - return the catalog of this component's events");

    /// <summary>
    /// Information: "Order {OrderId} accepted", event 1001 named OrderAccepted.
    /// </summary>
    public static void LogOrderAccepted(ILogger logger, string orderId) =>
        throw new NotImplementedException("TODO: Ex011 - write the OrderAccepted event");

    /// <summary>
    /// Warning: "Order {OrderId} rejected: {Reason}", event 1002 named OrderRejected.
    /// </summary>
    public static void LogOrderRejected(ILogger logger, string orderId, string reason) =>
        throw new NotImplementedException("TODO: Ex011 - write the OrderRejected event");

    /// <summary>
    /// Information: "Payment for {OrderId} retried, attempt {Attempt}", event 1003
    /// named PaymentRetried.
    /// </summary>
    public static void LogPaymentRetried(ILogger logger, string orderId, int attempt) =>
        throw new NotImplementedException("TODO: Ex011 - write the PaymentRetried event");
}
