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

// Exercise 055 — CorrelationContextPropagation (cross-cutting).
// Goal:   Carry a correlation id from the code that starts a piece of work to the code
//         that finishes it, across a boundary that shares no memory.
// Drills: correlation ids, message metadata, why ambient context stops at the wire.
// Passes: Enrich   - puts the id in the message's headers under HeaderName, and keeps
//                    every header that was already there.
//         Extract  - reads it back out, and returns null when there is none.
//         Continue - an incoming id is CONTINUED unchanged; a missing one starts a fresh
//                    non-empty id.
//         THE ONE   - a consumer running on a DIFFERENT THREAD, with AmbientCorrelation
//                    empty, still recovers the producer's id from the message.
//
// The cross-thread fact is the exercise. AsyncLocal is the natural reach - it is what
// ILogger scopes and Activity.Current use, and inside one process it works - and it is
// carried by the execution context, which does not survive being serialised onto a
// queue. The consumer is a different process on a different machine an hour later, and
// its ambient context is empty and always will be. Anything that must cross that
// boundary has to be IN THE MESSAGE.
public static class Ex055_CorrelationContextPropagation
{
    public const string HeaderName = "x-correlation-id";

    public static BusMessage Enrich(string payload, string correlationId, IReadOnlyDictionary<string, string>? existingHeaders = null) =>
        throw new NotImplementedException(
            "TODO: Ex055 - build a message whose headers carry the correlation id alongside anything already there");

    public static string? Extract(BusMessage message) =>
        throw new NotImplementedException("TODO: Ex055 - read the correlation id out of the headers, or null");

    /// <summary>Continue an incoming correlation, or start a new one.</summary>
    public static string Continue(string? incoming) =>
        throw new NotImplementedException(
            "TODO: Ex055 - return the incoming id unchanged when there is one, otherwise a fresh non-empty id");
}
