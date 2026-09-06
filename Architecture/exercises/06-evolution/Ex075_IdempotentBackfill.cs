namespace FeWoLearning.Architecture.Exercises.Evolution.Ex075;

public sealed record Row(string Id, string? Normalised, string Raw);

public sealed record BackfillResult(
    int Repaired,
    int Skipped,
    IReadOnlyList<string> Failed,
    string? ResumeAfter,
    bool Complete);

/// <summary>The table being repaired. Rows are ordered by Id, which is what makes resuming possible.</summary>
public sealed class RepairTable(IEnumerable<Row> rows)
{
    private readonly List<Row> _rows = [.. rows.OrderBy(r => r.Id, StringComparer.Ordinal)];

    public IReadOnlyList<Row> Rows => _rows;

    public IReadOnlyList<Row> Page(string? after, int size) =>
        [.. _rows.Where(r => after is null || string.CompareOrdinal(r.Id, after) > 0).Take(size)];

    public void Update(string id, string normalised)
    {
        var index = _rows.FindIndex(r => r.Id == id);
        _rows[index] = _rows[index] with { Normalised = normalised };
    }
}

// Exercise 075 — IdempotentBackfill (evolution).
// Goal:   Repair millions of rows in a running system, in bites, with the option of
//         stopping and starting again.
// Drills: batching, checkpoints, idempotence, one bad row not stopping the rest.
// Passes: batching  - a run limited to maxBatches touches only that many pages and comes
//                     back with Complete false and a ResumeAfter.
//         resuming  - the next run starts AFTER that id, not at the beginning.
//         THE ONE    - re-running over rows that are already repaired changes nothing and
//                     counts them as Skipped rather than Repaired. A backfill is going to
//                     be run more than once - it will be interrupted, or somebody will be
//                     unsure whether it finished.
//         failure   - a row whose repair throws is recorded and the run CONTINUES. One
//                     malformed row out of ten million must not stop the other
//                     9,999,999.
//         finishing - the last run reports Complete and no ResumeAfter.
//
// A backfill is a migration that cannot be a migration: too big for one transaction, too
// slow for a deployment window, and running against a database that is simultaneously
// serving traffic. Everything here follows from that. It goes in pages so it can be
// throttled, it checkpoints so it can be stopped, it skips finished work so it can be
// re-run without thinking, and it survives bad rows because in ten million of them there
// are bad rows.
//
// The Skipped count is not decoration. "Repaired: 0, Skipped: 9,999,998, Failed: 2" is a
// finished backfill with two rows to look at; "Repaired: 0" on its own is
// indistinguishable from a backfill that silently did nothing.
public static class Ex075_IdempotentBackfill
{
    /// <summary>
    /// Repair up to <paramref name="maxBatches"/> pages of <paramref name="batchSize"/>
    /// rows, starting after <paramref name="resumeAfter"/>. A row needs repair when its
    /// Normalised value is null; <paramref name="normalise"/> produces it from Raw and may
    /// throw.
    /// </summary>
    public static BackfillResult Run(
        RepairTable table,
        string? resumeAfter,
        int batchSize,
        int maxBatches,
        Func<string, string> normalise) =>
        throw new NotImplementedException(
            "TODO: Ex075 - page through the table from the checkpoint, repair only the rows that need it, record failures without stopping, and report where to resume");
}
