namespace FeWoLearning.Architecture.Exercises.Desktop.Ex028;

/// <summary>One named value on an event. Individually addressable, on purpose.</summary>
public readonly record struct LogField(string Name, object? Value);

/// <summary>
/// The telemetry port. The domain depends on this and on nothing else - no ILogger, no
/// sink, no format string. What the fields become downstream is somebody else's problem.
/// </summary>
public interface ITelemetry
{
    void Record(string eventName, IReadOnlyList<LogField> fields);
}

public sealed record Order(string Id, decimal Amount, string Customer);

// Exercise 028 — TelemetryBoundary (desktop).
// Goal:   Emit an event the machine can read, from a domain that has never heard of a
//         logging framework.
// Drills: logging as a port, structured fields, keeping formatting out of the domain.
// Passes: Place() - records exactly ONE event, named "order.placed", carrying exactly
//                   three fields: "orderId" (the string), "amount" (the DECIMAL, not a
//                   rendering of it) and "customer" (the string).
//
// "The decimal, not a rendering of it" is the fact that matters. The tempting version is
// Record("order placed: {orderId} for {amount:C}"), which reads beautifully in a console
// and is useless the moment somebody wants to sum the amounts, alert on orders above a
// threshold, or group by customer - all of which are now string parsing against a format
// that changes with the machine's culture. Rendering is a decision for the sink, and the
// sink is not here.
public sealed class OrderService(ITelemetry telemetry)
{
    public void Place(Order order) =>
        throw new NotImplementedException(
            "TODO: Ex028 - record one \"order.placed\" event with orderId, amount and customer as three separate, unformatted fields");
}
