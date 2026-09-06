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

// Exercise 042 — EventSourcingAppendStream (services-data).
// Goal:   Store what HAPPENED rather than what is, and refuse a writer who was looking
//         at an older version of the story.
// Drills: append-only streams, rehydration, expected-version check.
// Passes: Append/Read - events come back in the order they were appended.
//         version     - advances by exactly the number of events appended; an unknown
//                       stream is at version 0 and reads empty.
//         THE ONE      - appending with a stale expected version throws
//                       ConcurrencyException AND leaves the stream completely unchanged.
//         Rehydrate   - folds the events into the current balance.
//         append-only - a later append never alters what was already there.
//
// Rehydration is what makes the append-only rule affordable: the balance is not stored
// anywhere, it is computed from the events every time. Which is also the fact that
// catches the shortcut - an implementation that keeps a running total and hands it back
// passes every balance assertion and has stopped being event-sourced, because the
// events are now a log nobody reads and nothing keeps honest.
public sealed class EventStore
{
    /// <summary>
    /// Append <paramref name="events"/> to <paramref name="streamId"/>, but only if the
    /// stream is still at <paramref name="expectedVersion"/>. Returns the new version.
    /// </summary>
    public int Append(string streamId, int expectedVersion, IReadOnlyList<IEvent> events) =>
        throw new NotImplementedException(
            "TODO: Ex042 - reject a stale expected version without changing anything, otherwise append and return the new version");

    public IReadOnlyList<IEvent> Read(string streamId) =>
        throw new NotImplementedException("TODO: Ex042 - every event in this stream, oldest first");

    public int VersionOf(string streamId) =>
        throw new NotImplementedException("TODO: Ex042 - how many events this stream holds");
}

public static class Ex042_EventSourcingAppendStream
{
    /// <summary>Compute the balance from the events. Nothing is stored.</summary>
    public static decimal Rehydrate(IReadOnlyList<IEvent> events) =>
        throw new NotImplementedException(
            "TODO: Ex042 - fold the events into a balance: deposits add, withdrawals subtract");
}
