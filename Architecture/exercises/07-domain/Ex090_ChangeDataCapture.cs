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

// Exercise 090 — ChangeDataCapture (domain).
// Goal:   Derive an event stream from what the database already recorded, and see clearly
//         what that stream can and cannot say.
// Drills: log-based capture, translating rows to events, checkpoints, what CDC loses.
// Passes: insert    - becomes a "<table>.created" event carrying the new row.
//         delete    - becomes a "<table>.deleted" event carrying the row that was there.
//         update    - becomes a "<table>.updated" event carrying ONLY THE CHANGED COLUMNS.
//                     A consumer that has to diff two full rows itself is being handed the
//                     work the capture was supposed to do.
//         THE ONE    - an update that changes nothing produces NO event. Row-level capture
//                     sees every write, including the ones that rewrite identical values,
//                     and a stream full of "nothing happened" is one every consumer has to
//                     filter - each of them slightly differently.
//         tables    - only the tables asked for are translated.
//         resuming  - reading from an LSN yields only changes after it, and the returned
//                     checkpoint is the highest LSN seen.
//
// CDC is the pattern for getting events out of a system that was never designed to emit
// them, and its honest limitation is worth stating: it captures what CHANGED, never why.
// "status went from 'pending' to 'cancelled'" is recoverable from the log;
// "the customer cancelled" versus "the payment expired" is not, because the database never
// knew. An outbox (exercise 032) carries the intent because the code that had the intent
// wrote the message; CDC infers a shadow of it afterwards.
//
// Which is the trade: an outbox needs the writing service to cooperate, and CDC does not
// need it to know anything at all. For a legacy system nobody is allowed to change, that
// is the difference between having events and not.
public static class Ex090_ChangeDataCapture
{
    /// <summary>
    /// Translate the changes after <paramref name="fromLsn"/> for the tables in
    /// <paramref name="capturedTables"/>. Returns the events and the new checkpoint.
    /// </summary>
    public static (IReadOnlyList<DomainEvent> Events, long Checkpoint) Capture(
        IReadOnlyList<RowChange> log,
        long fromLsn,
        IReadOnlySet<string> capturedTables,
        string idColumn) =>
        throw new NotImplementedException(
            "TODO: Ex090 - translate inserts, deletes and REAL updates into events, skipping no-op updates and uncaptured tables, and report the highest LSN seen");
}
