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

// Exercise 028 — TelemetryBoundary (reference solution).
public sealed class OrderService(ITelemetry telemetry)
{
    public void Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        // order.Amount goes through as a decimal. Formatting it here would bake this
        // machine's culture into the record - and a sum over "1.234,56" and "1,234.56"
        // is not a sum, it is an incident.
        telemetry.Record("order.placed",
        [
            new LogField("orderId", order.Id),
            new LogField("amount", order.Amount),
            new LogField("customer", order.Customer),
        ]);
    }
}
