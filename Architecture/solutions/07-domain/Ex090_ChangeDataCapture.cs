namespace FeWoLearning.Architecture.Exercises.Domain.Ex090;

public enum ChangeKind
{
    Insert,
    Update,
    Delete,
}

/// <summary>
/// One row change as the log records it. Before and After are the row's columns; one of
/// them is null at each end of a row's life.
/// </summary>
public sealed record RowChange(
    long Lsn,
    string Table,
    ChangeKind Kind,
    IReadOnlyDictionary<string, string>? Before,
    IReadOnlyDictionary<string, string>? After);

public sealed record DomainEvent(string Type, string EntityId, IReadOnlyDictionary<string, string> Data, long Lsn);

// Exercise 090 — ChangeDataCapture (reference solution).
public static class Ex090_ChangeDataCapture
{
    public static (IReadOnlyList<DomainEvent> Events, long Checkpoint) Capture(
        IReadOnlyList<RowChange> log,
        long fromLsn,
        IReadOnlySet<string> capturedTables,
        string idColumn)
    {
        var events = new List<DomainEvent>();
        var checkpoint = fromLsn;

        foreach (var change in log.Where(c => c.Lsn > fromLsn).OrderBy(c => c.Lsn))
        {
            // The checkpoint advances for EVERY change, including the ones that produce no
            // event. Advancing only on emitted events means a long run of writes to
            // uncaptured tables is re-read on every poll, for ever.
            checkpoint = change.Lsn;

            if (!capturedTables.Contains(change.Table))
                continue;

            switch (change.Kind)
            {
                case ChangeKind.Insert when change.After is { } inserted:
                    events.Add(new DomainEvent($"{change.Table}.created", inserted[idColumn], inserted, change.Lsn));
                    break;

                case ChangeKind.Delete when change.Before is { } deleted:
                    events.Add(new DomainEvent($"{change.Table}.deleted", deleted[idColumn], deleted, change.Lsn));
                    break;

                case ChangeKind.Update when change.Before is { } before && change.After is { } after:
                    var changed = after
                        .Where(kv => !before.TryGetValue(kv.Key, out var old) || old != kv.Value)
                        .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

                    // No event for a write that changed nothing. Row-level capture sees
                    // every write, including the ones rewriting identical values, and a
                    // stream full of "nothing happened" is one every consumer has to
                    // filter - each of them slightly differently.
                    if (changed.Count == 0)
                        break;

                    events.Add(new DomainEvent($"{change.Table}.updated", after[idColumn], changed, change.Lsn));
                    break;
            }
        }

        return (events, checkpoint);
    }
}
