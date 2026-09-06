namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex055;

public sealed record BusMessage(string Payload, IReadOnlyDictionary<string, string> Headers);

/// <summary>
/// The tempting mechanism, shipped so the exercise can show what it cannot do. An
/// AsyncLocal follows the async flow beautifully - within one process, within one call
/// chain - and is completely empty on the other side of a queue.
/// </summary>
public static class AmbientCorrelation
{
    private static readonly AsyncLocal<string?> Current = new();

    public static string? Value
    {
        get => Current.Value;
        set => Current.Value = value;
    }
}

// Exercise 055 — CorrelationContextPropagation (reference solution).
public static class Ex055_CorrelationContextPropagation
{
    public const string HeaderName = "x-correlation-id";

    public static BusMessage Enrich(string payload, string correlationId, IReadOnlyDictionary<string, string>? existingHeaders = null)
    {
        // Copied, not mutated, and the existing headers survive. A producer that replaces
        // the header dictionary drops the tenant id, the schema version and everything
        // else the transport was carrying.
        var headers = existingHeaders is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(existingHeaders, StringComparer.OrdinalIgnoreCase);

        headers[HeaderName] = correlationId;

        // In the MESSAGE. Not in an AsyncLocal - that is carried by the execution
        // context, which does not survive being serialised onto a queue, and the consumer
        // is a different process on a different machine an hour later.
        return new BusMessage(payload, headers);
    }

    public static string? Extract(BusMessage message) =>
        message.Headers.TryGetValue(HeaderName, out var id) && !string.IsNullOrWhiteSpace(id) ? id : null;

    public static string Continue(string? incoming) =>
        // Continued, not regenerated. A new id per hop is a set of unrelated traces, and
        // the one question correlation exists to answer - "what else happened because of
        // this request" - becomes unanswerable.
        string.IsNullOrWhiteSpace(incoming) ? Guid.NewGuid().ToString("N") : incoming;
}
