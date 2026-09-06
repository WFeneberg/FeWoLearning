using FeWoLearning.Architecture.Exercises.ServicesData.Ex043;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex043_EventSourcedProjectionTests
{
    private static List<LogEntry> Log() =>
    [
        new(1, "acc-1", 100m),
        new(2, "acc-2", 50m),
        new(3, "acc-1", -30m),
    ];

    [Fact]
    public void Catching_Up_Applies_Everything_And_Moves_The_Checkpoint()
    {
        var projection = new BalanceProjection();

        projection.CatchUp(Log());

        Assert.Equal(70m, projection.Balances["acc-1"]);
        Assert.Equal(50m, projection.Balances["acc-2"]);
        Assert.Equal(3, projection.Checkpoint);
    }

    [Fact]
    public void Mechanism_Catching_Up_Twice_On_The_Same_Log_Changes_Nothing()
    {
        // The fact this exercise exists for. A projection that applies whatever it is
        // handed produces the right answer the first time and doubles every balance the
        // second - and it WILL be handed the same entries twice, because everything
        // feeding it is at-least-once.
        var projection = new BalanceProjection();
        var log = Log();

        projection.CatchUp(log);
        projection.CatchUp(log);

        Assert.Equal(70m, projection.Balances["acc-1"]);
        Assert.Equal(50m, projection.Balances["acc-2"]);
        Assert.Equal(3, projection.Checkpoint);
    }

    [Fact]
    public void Mechanism_A_Second_Catch_Up_Applies_Only_The_New_Entries()
    {
        // The other half of the checkpoint's job: resuming rather than rebuilding. An
        // implementation that reapplies the whole log every time gives the right answer
        // only because the fact above forced it to be idempotent - and pays for it with
        // work proportional to all of history on every poll.
        var projection = new BalanceProjection();
        var log = Log();
        projection.CatchUp(log);

        log.Add(new LogEntry(4, "acc-1", 5m));
        projection.CatchUp(log);

        Assert.Equal(75m, projection.Balances["acc-1"]);
        Assert.Equal(4, projection.Checkpoint);
    }

    [Fact]
    public void Adversarial_An_Entry_At_Or_Below_The_Checkpoint_Is_Skipped()
    {
        // A redelivery of one old entry, mixed in with new ones - which is exactly what
        // a resubscribing consumer sees. Filtering on "not seen this position before"
        // rather than "above the checkpoint" would need unbounded memory to do the same
        // job.
        var projection = new BalanceProjection();
        projection.CatchUp(Log());

        projection.CatchUp([new LogEntry(2, "acc-2", 50m), new LogEntry(4, "acc-2", 1m)]);

        Assert.Equal(51m, projection.Balances["acc-2"]);
        Assert.Equal(4, projection.Checkpoint);
    }

    [Fact]
    public void Entries_Are_Applied_In_Position_Order_However_They_Arrive()
    {
        var projection = new BalanceProjection();

        projection.CatchUp([new LogEntry(3, "acc-1", -30m), new LogEntry(1, "acc-1", 100m), new LogEntry(2, "acc-2", 50m)]);

        Assert.Equal(70m, projection.Balances["acc-1"]);
        Assert.Equal(3, projection.Checkpoint);
    }

    [Fact]
    public void A_Rebuild_From_Scratch_Agrees_With_An_Incremental_Catch_Up()
    {
        // The property that makes a read model disposable: it can always be thrown away
        // and rebuilt from the log, and must land in the same place.
        var incremental = new BalanceProjection();
        var log = Log();
        incremental.CatchUp([log[0]]);
        incremental.CatchUp([log[0], log[1]]);
        incremental.CatchUp(log);

        var rebuilt = new BalanceProjection();
        rebuilt.CatchUp(log);

        Assert.Equal(rebuilt.Balances, incremental.Balances);
        Assert.Equal(rebuilt.Checkpoint, incremental.Checkpoint);
    }
}
