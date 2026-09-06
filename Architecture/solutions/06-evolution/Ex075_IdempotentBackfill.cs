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

// Exercise 075 — IdempotentBackfill (reference solution).
public static class Ex075_IdempotentBackfill
{
    public static BackfillResult Run(
        RepairTable table,
        string? resumeAfter,
        int batchSize,
        int maxBatches,
        Func<string, string> normalise)
    {
        var repaired = 0;
        var skipped = 0;
        var failed = new List<string>();
        var cursor = resumeAfter;
        var complete = false;

        for (var batch = 0; batch < maxBatches; batch++)
        {
            var page = table.Page(cursor, batchSize);

            if (page.Count == 0)
            {
                complete = true;
                break;
            }

            foreach (var row in page)
            {
                // The checkpoint moves for EVERY row, repaired or not. Advancing only past
                // repaired rows means a resumed run re-reads every already-good row before
                // it reaches new work - and on the second half of a large table that is
                // most of the run.
                cursor = row.Id;

                if (row.Normalised is not null)
                {
                    // Counted, not ignored. "Repaired: 0, Skipped: 9,999,998, Failed: 2" is
                    // a finished backfill with two rows to look at; "Repaired: 0" on its
                    // own is indistinguishable from one that silently did nothing.
                    skipped++;
                    continue;
                }

                try
                {
                    table.Update(row.Id, normalise(row.Raw));
                    repaired++;
                }
                catch
                {
                    // Recorded, and the run continues. One malformed row out of ten million
                    // must not stop the other 9,999,999 - and it must not stop the
                    // checkpoint either, or every resumed run stalls on the same row.
                    failed.Add(row.Id);
                }
            }

            if (page.Count < batchSize)
            {
                complete = true;
                break;
            }
        }

        return new BackfillResult(repaired, skipped, failed, complete ? null : cursor, complete);
    }
}
