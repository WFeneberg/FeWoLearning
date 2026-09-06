namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex042;

public interface IEvent;

public sealed record Deposited(decimal Amount) : IEvent;

public sealed record Withdrawn(decimal Amount) : IEvent;

/// <summary>Thrown when the caller's expected version no longer matches the stream.</summary>
public sealed class ConcurrencyException(string streamId, int expected, int actual)
    : Exception($"Stream '{streamId}' is at version {actual}, not {expected}.")
{
    public int Expected { get; } = expected;
    public int Actual { get; } = actual;
}

// Exercise 042 — EventSourcingAppendStream (reference solution).
public sealed class EventStore
{
    private readonly Dictionary<string, List<IEvent>> _streams = [];

    public int Append(string streamId, int expectedVersion, IReadOnlyList<IEvent> events)
    {
        var stream = _streams.TryGetValue(streamId, out var existing) ? existing : [];

        // Checked BEFORE anything is written. Appending first and validating afterwards
        // leaves the rejected events in the stream, and an event store that contains
        // things that did not happen is worse than no event store.
        if (stream.Count != expectedVersion)
            throw new ConcurrencyException(streamId, expectedVersion, stream.Count);

        stream.AddRange(events);
        _streams[streamId] = stream;
        return stream.Count;
    }

    public IReadOnlyList<IEvent> Read(string streamId) =>
        _streams.TryGetValue(streamId, out var stream) ? [.. stream] : [];

    public int VersionOf(string streamId) =>
        _streams.TryGetValue(streamId, out var stream) ? stream.Count : 0;
}

public static class Ex042_EventSourcingAppendStream
{
    // Computed, never stored. Keeping a running total instead passes every balance
    // assertion and stops the system being event-sourced: the events become a log
    // nobody reads and nothing keeps honest.
    public static decimal Rehydrate(IReadOnlyList<IEvent> events) =>
        events.Aggregate(0m, (balance, e) => e switch
        {
            Deposited deposited => balance + deposited.Amount,
            Withdrawn withdrawn => balance - withdrawn.Amount,
            _ => balance,
        });
}
