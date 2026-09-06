namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex043;

/// <summary>
/// One entry in the global log. Position is the thing the checkpoint refers to, and it
/// is why a projection can be resumed rather than only rebuilt.
/// </summary>
public sealed record LogEntry(long Position, string AccountId, decimal Delta);

// Exercise 043 — EventSourcedProjection (services-data).
// Goal:   Build a read model by catching up on an event log, and make catching up
//         resumable and safe to repeat.
// Drills: catch-up projection, checkpoints, idempotent apply.
// Passes: first catch-up  - every entry is applied and Checkpoint ends at the last
//                           position seen.
//         THE ONE          - running catch-up AGAIN over the SAME log changes nothing:
//                           the balances are identical and the checkpoint has not moved.
//         resuming         - a second catch-up over a log that has grown applies ONLY the
//                           new entries.
//         out-of-order     - an entry at or below the checkpoint is skipped, so the
//                           checkpoint never goes backwards.
//         a fresh projection rebuilt from the whole log agrees with an incrementally
//                           caught-up one.
//
// The checkpoint is the whole exercise, and "run it twice" is the fact that grades it.
// A projection that simply applies whatever it is handed produces the right answer the
// first time and doubles every balance the second - and it WILL be handed the same
// entries twice, because the thing feeding it is at-least-once (see 032 and 033). This
// is the point at which a read model either survives a redelivery or silently becomes
// fiction.
public sealed class BalanceProjection
{
    public IReadOnlyDictionary<string, decimal> Balances =>
        throw new NotImplementedException("TODO: Ex043 - the projected balance per account");

    /// <summary>The highest position this projection has applied. Starts at 0.</summary>
    public long Checkpoint =>
        throw new NotImplementedException("TODO: Ex043 - the highest applied position");

    /// <summary>Apply every entry after the current checkpoint, in position order.</summary>
    public void CatchUp(IReadOnlyList<LogEntry> log) =>
        throw new NotImplementedException(
            "TODO: Ex043 - apply only entries whose position is above the checkpoint, in order, and move the checkpoint with them");
}
