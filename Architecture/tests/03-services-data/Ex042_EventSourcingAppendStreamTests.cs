using FeWoLearning.Architecture.Exercises.ServicesData.Ex042;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex042_EventSourcingAppendStreamTests
{
    [Fact]
    public void Events_Come_Back_In_The_Order_They_Were_Appended()
    {
        var store = new EventStore();

        store.Append("acc-1", 0, [new Deposited(100m), new Withdrawn(30m)]);

        Assert.Equal([new Deposited(100m), new Withdrawn(30m)], store.Read("acc-1"));
    }

    [Fact]
    public void An_Unknown_Stream_Is_Empty_And_At_Version_Zero()
    {
        var store = new EventStore();

        Assert.Empty(store.Read("never-written"));
        Assert.Equal(0, store.VersionOf("never-written"));
    }

    [Fact]
    public void The_Version_Advances_By_The_Number_Of_Events()
    {
        var store = new EventStore();

        Assert.Equal(2, store.Append("acc-1", 0, [new Deposited(100m), new Withdrawn(30m)]));
        Assert.Equal(3, store.Append("acc-1", 2, [new Deposited(5m)]));
        Assert.Equal(3, store.VersionOf("acc-1"));
    }

    [Fact]
    public void Mechanism_A_Stale_Expected_Version_Is_Rejected()
    {
        var store = new EventStore();
        store.Append("acc-1", 0, [new Deposited(100m)]);

        var failure = Assert.Throws<ConcurrencyException>(
            () => store.Append("acc-1", 0, [new Withdrawn(30m)]));

        Assert.Equal(0, failure.Expected);
        Assert.Equal(1, failure.Actual);
    }

    [Fact]
    public void Mechanism_A_Rejected_Append_Leaves_The_Stream_Untouched()
    {
        // Appending first and validating afterwards throws the same exception and leaves
        // the rejected events in the stream. An event store containing things that did
        // not happen is worse than no event store: every projection built from it is now
        // wrong, and rebuilding cannot fix it.
        var store = new EventStore();
        store.Append("acc-1", 0, [new Deposited(100m)]);

        Assert.Throws<ConcurrencyException>(() => store.Append("acc-1", 0, [new Withdrawn(30m)]));

        Assert.Equal([new Deposited(100m)], store.Read("acc-1"));
        Assert.Equal(1, store.VersionOf("acc-1"));
    }

    [Fact]
    public void Appending_Never_Alters_What_Was_Already_There()
    {
        var store = new EventStore();
        store.Append("acc-1", 0, [new Deposited(100m)]);

        store.Append("acc-1", 1, [new Withdrawn(30m)]);

        Assert.Equal(new Deposited(100m), store.Read("acc-1")[0]);
    }

    [Fact]
    public void Mechanism_The_Balance_Is_Computed_From_The_Events()
    {
        // Nothing stores a balance. An implementation that kept a running total would
        // pass this and stop being event-sourced - the events would become a log nobody
        // reads and nothing keeps honest.
        var store = new EventStore();
        store.Append("acc-1", 0, [new Deposited(100m), new Withdrawn(30m), new Deposited(5m)]);

        Assert.Equal(75m, Ex042_EventSourcingAppendStream.Rehydrate(store.Read("acc-1")));
    }

    [Fact]
    public void Streams_Are_Independent()
    {
        var store = new EventStore();

        store.Append("acc-1", 0, [new Deposited(100m)]);
        store.Append("acc-2", 0, [new Deposited(7m)]);

        Assert.Equal(100m, Ex042_EventSourcingAppendStream.Rehydrate(store.Read("acc-1")));
        Assert.Equal(7m, Ex042_EventSourcingAppendStream.Rehydrate(store.Read("acc-2")));
    }
}
