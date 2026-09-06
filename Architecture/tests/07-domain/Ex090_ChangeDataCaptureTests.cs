using FeWoLearning.Architecture.Exercises.Domain.Ex090;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex090_ChangeDataCaptureTests
{
    private static readonly HashSet<string> Captured = ["orders"];

    private static Dictionary<string, string> Row(string id, string status, string total = "100") =>
        new(StringComparer.Ordinal) { ["id"] = id, ["status"] = status, ["total"] = total };

    private static (IReadOnlyList<DomainEvent> Events, long Checkpoint) Capture(
        IReadOnlyList<RowChange> log, long fromLsn = 0) =>
        Ex090_ChangeDataCapture.Capture(log, fromLsn, Captured, "id");

    [Fact]
    public void An_Insert_Becomes_A_Created_Event()
    {
        var (events, _) = Capture([new RowChange(1, "orders", ChangeKind.Insert, null, Row("o-1", "pending"))]);

        var created = Assert.Single(events);
        Assert.Equal("orders.created", created.Type);
        Assert.Equal("o-1", created.EntityId);
        Assert.Equal("pending", created.Data["status"]);
    }

    [Fact]
    public void A_Delete_Becomes_A_Deleted_Event_Carrying_What_Was_There()
    {
        // The row is gone; the event is the only place its contents survive. An event
        // carrying just the id makes every consumer's own copy unrecoverable.
        var (events, _) = Capture([new RowChange(1, "orders", ChangeKind.Delete, Row("o-1", "cancelled"), null)]);

        var deleted = Assert.Single(events);
        Assert.Equal("orders.deleted", deleted.Type);
        Assert.Equal("cancelled", deleted.Data["status"]);
    }

    [Fact]
    public void Mechanism_An_Update_Carries_Only_What_Changed()
    {
        // A consumer that has to diff two full rows itself is being handed the work the
        // capture was supposed to do - and every consumer will implement that diff
        // slightly differently.
        var (events, _) = Capture(
        [
            new RowChange(1, "orders", ChangeKind.Update, Row("o-1", "pending"), Row("o-1", "shipped")),
        ]);

        var updated = Assert.Single(events);
        Assert.Equal("orders.updated", updated.Type);
        Assert.Equal("o-1", updated.EntityId);
        Assert.Equal(["status"], updated.Data.Keys);
        Assert.Equal("shipped", updated.Data["status"]);
    }

    [Fact]
    public void Mechanism_An_Update_That_Changed_Nothing_Produces_No_Event()
    {
        // Row-level capture sees every write, including the ones that rewrite identical
        // values - a batch job that touches every row nightly, an ORM that updates all
        // columns whether or not they changed. A stream full of "nothing happened" is one
        // every consumer has to filter.
        var (events, checkpoint) = Capture(
        [
            new RowChange(1, "orders", ChangeKind.Update, Row("o-1", "pending"), Row("o-1", "pending")),
        ]);

        Assert.Empty(events);
        Assert.Equal(1, checkpoint);
    }

    [Fact]
    public void Only_Captured_Tables_Are_Translated()
    {
        var (events, _) = Capture(
        [
            new RowChange(1, "orders", ChangeKind.Insert, null, Row("o-1", "pending")),
            new RowChange(2, "audit_log", ChangeKind.Insert, null, Row("a-1", "whatever")),
        ]);

        Assert.Single(events);
        Assert.Equal("orders.created", events[0].Type);
    }

    [Fact]
    public void Reading_From_A_Checkpoint_Skips_What_Came_Before()
    {
        var log = new[]
        {
            new RowChange(1, "orders", ChangeKind.Insert, null, Row("o-1", "pending")),
            new RowChange(2, "orders", ChangeKind.Update, Row("o-1", "pending"), Row("o-1", "shipped")),
        };

        var (events, checkpoint) = Capture(log, fromLsn: 1);

        Assert.Single(events);
        Assert.Equal("orders.updated", events[0].Type);
        Assert.Equal(2, checkpoint);
    }

    [Fact]
    public void Adversarial_The_Checkpoint_Advances_Past_Changes_That_Emitted_Nothing()
    {
        // Advancing only on EMITTED events means a long run of writes to uncaptured
        // tables - an audit log, a session table - is re-read on every poll, for ever, and
        // the lag grows with the tables nobody cares about.
        var (events, checkpoint) = Capture(
        [
            new RowChange(1, "audit_log", ChangeKind.Insert, null, Row("a-1", "x")),
            new RowChange(2, "audit_log", ChangeKind.Insert, null, Row("a-2", "x")),
            new RowChange(3, "sessions", ChangeKind.Update, Row("s-1", "live"), Row("s-1", "live")),
        ]);

        Assert.Empty(events);
        Assert.Equal(3, checkpoint);
    }

    [Fact]
    public void Changes_Are_Emitted_In_Log_Order_However_They_Arrive()
    {
        // The LSN is the order the database committed them in, and it is the only ordering
        // that means anything downstream.
        var (events, _) = Capture(
        [
            new RowChange(3, "orders", ChangeKind.Update, Row("o-1", "shipped"), Row("o-1", "delivered")),
            new RowChange(1, "orders", ChangeKind.Insert, null, Row("o-1", "pending")),
            new RowChange(2, "orders", ChangeKind.Update, Row("o-1", "pending"), Row("o-1", "shipped")),
        ]);

        Assert.Equal(["orders.created", "orders.updated", "orders.updated"], events.Select(e => e.Type));
        Assert.Equal([1L, 2L, 3L], events.Select(e => e.Lsn));
    }
}
