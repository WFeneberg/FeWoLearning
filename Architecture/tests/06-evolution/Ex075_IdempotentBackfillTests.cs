using FeWoLearning.Architecture.Exercises.Evolution.Ex075;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex075_IdempotentBackfillTests
{
    private static RepairTable Table(int count = 10) =>
        new(Enumerable.Range(1, count).Select(i => new Row($"row-{i:D3}", null, $"  RAW-{i}  ")));

    private static string Normalise(string raw) => raw.Trim().ToLowerInvariant();

    [Fact]
    public void A_Complete_Run_Repairs_Everything()
    {
        var table = Table();

        var result = Ex075_IdempotentBackfill.Run(table, null, batchSize: 4, maxBatches: 10, Normalise);

        Assert.Equal(10, result.Repaired);
        Assert.True(result.Complete);
        Assert.Null(result.ResumeAfter);
        Assert.All(table.Rows, r => Assert.NotNull(r.Normalised));
    }

    [Fact]
    public void Mechanism_A_Limited_Run_Stops_And_Says_Where()
    {
        // A backfill is too big for one transaction and too slow for a deployment window,
        // and it runs against a database that is simultaneously serving traffic. Being
        // able to stop is not a nicety.
        var table = Table();

        var result = Ex075_IdempotentBackfill.Run(table, null, batchSize: 3, maxBatches: 2, Normalise);

        Assert.Equal(6, result.Repaired);
        Assert.False(result.Complete);
        Assert.Equal("row-006", result.ResumeAfter);
        Assert.Equal(4, table.Rows.Count(r => r.Normalised is null));
    }

    [Fact]
    public void Mechanism_Resuming_Starts_After_The_Checkpoint()
    {
        var table = Table();
        var first = Ex075_IdempotentBackfill.Run(table, null, 3, 2, Normalise);

        var second = Ex075_IdempotentBackfill.Run(table, first.ResumeAfter, 3, 10, Normalise);

        Assert.Equal(4, second.Repaired);
        Assert.Equal(0, second.Skipped);   // it did not walk back over the finished rows
        Assert.True(second.Complete);
    }

    [Fact]
    public void Mechanism_Re_Running_From_The_Start_Repairs_Nothing_And_Says_So()
    {
        // It WILL be run more than once: it gets interrupted, or somebody is unsure
        // whether it finished. The Skipped count is what makes the second run's report
        // legible - "Repaired: 0" on its own is indistinguishable from a backfill that
        // silently did nothing.
        var table = Table();
        Ex075_IdempotentBackfill.Run(table, null, 4, 10, Normalise);

        var again = Ex075_IdempotentBackfill.Run(table, null, 4, 10, Normalise);

        Assert.Equal(0, again.Repaired);
        Assert.Equal(10, again.Skipped);
        Assert.True(again.Complete);
        Assert.Equal("raw-1", table.Rows[0].Normalised);
    }

    [Fact]
    public void Adversarial_Re_Running_Does_Not_Overwrite_A_Repaired_Row()
    {
        // A backfill that recomputes unconditionally undoes anything the live system has
        // changed since - and looks perfectly fine afterwards, because every row has a
        // value.
        var table = Table(3);
        Ex075_IdempotentBackfill.Run(table, null, 10, 10, Normalise);
        table.Update("row-001", "edited-by-the-application");

        Ex075_IdempotentBackfill.Run(table, null, 10, 10, Normalise);

        Assert.Equal("edited-by-the-application", table.Rows[0].Normalised);
    }

    [Fact]
    public void Mechanism_A_Failing_Row_Is_Recorded_And_The_Run_Continues()
    {
        // One malformed row out of ten million must not stop the other 9,999,999 - and it
        // must not stop the checkpoint either, or every resumed run stalls on the same row
        // for ever.
        var table = Table(5);

        var result = Ex075_IdempotentBackfill.Run(table, null, 10, 10, raw =>
            raw.Contains("RAW-3", StringComparison.Ordinal)
                ? throw new FormatException("cannot normalise")
                : Normalise(raw));

        Assert.Equal(["row-003"], result.Failed);
        Assert.Equal(4, result.Repaired);
        Assert.True(result.Complete);
        Assert.All(table.Rows.Where(r => r.Id != "row-003"), r => Assert.NotNull(r.Normalised));
    }

    [Fact]
    public void Adversarial_A_Failing_Row_Does_Not_Hold_The_Checkpoint_Back()
    {
        // Advancing the checkpoint only past REPAIRED rows means a resumed run re-reads
        // every already-good row before reaching new work - and on the second half of a
        // large table, that is most of the run.
        //
        // The failing row is the LAST one this run touches, deliberately. With the failure
        // anywhere else, a checkpoint that only moves on success still ends up in the right
        // place - the rows after it repair and move it along - and the fact passes against
        // the wrong implementation. Measured while probing this batch.
        var table = Table(6);

        var result = Ex075_IdempotentBackfill.Run(table, null, 2, 2, raw =>
            raw.Contains("RAW-4", StringComparison.Ordinal)
                ? throw new FormatException("cannot normalise")
                : Normalise(raw));

        Assert.Equal(["row-004"], result.Failed);
        Assert.Equal("row-004", result.ResumeAfter);
        Assert.False(result.Complete);
    }

    [Fact]
    public void An_Empty_Table_Completes_Immediately()
    {
        var result = Ex075_IdempotentBackfill.Run(new RepairTable([]), null, 10, 10, Normalise);

        Assert.True(result.Complete);
        Assert.Equal(0, result.Repaired);
    }
}
