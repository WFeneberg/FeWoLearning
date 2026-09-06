using FeWoLearning.Architecture.Exercises.Scale.Ex071;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex071_ReadReplicaRoutingTests
{
    private static readonly TimeSpan Lag = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan Window = TimeSpan.FromSeconds(5);

    private static (ReadRouter Router, ReplicatedStore Store, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var store = new ReplicatedStore(clock, Lag);
        return (new ReadRouter(clock, store, Window), store, clock);
    }

    [Fact]
    public void Mechanism_A_Session_That_Has_Not_Written_Reads_From_The_Replica()
    {
        // The point of having replicas at all. A router that always reads the primary is
        // correct for every consistency fact below and has bought nothing.
        var (router, store, clock) = Build();
        router.Write("someone-else", "profile", "old");
        clock.Advance(Lag);

        var value = router.Read("reader", "profile");

        Assert.Equal("old", value);
        Assert.Equal(1, store.ReplicaReads);
        Assert.Equal(0, store.PrimaryReads);
    }

    [Fact]
    public void Mechanism_A_Session_Reads_Its_Own_Write_From_The_Primary()
    {
        // Eventual consistency is acceptable everywhere except where a user is looking at
        // the result of their own action. They will not accept it there - they will press
        // the button again, and the duplicate they create is a real one.
        var (router, store, _) = Build();

        router.Write("ada", "profile", "new");
        var value = router.Read("ada", "profile");

        Assert.Equal("new", value);
        Assert.Equal(1, store.PrimaryReads);
        Assert.Equal(0, store.ReplicaReads);
    }

    [Fact]
    public void Mechanism_The_Stickiness_Is_Per_Session()
    {
        // The fact that separates this from "somebody wrote recently, everybody reads the
        // primary". That version is correct, trivial, and on any system with more than one
        // active user means the primary serves everything - which is exactly the load the
        // replicas were bought to take.
        var (router, store, _) = Build();

        router.Write("ada", "profile", "new");

        router.Read("ada", "profile");        // sticky
        router.Read("grace", "profile");      // not sticky

        Assert.Equal(1, store.PrimaryReads);
        Assert.Equal(1, store.ReplicaReads);
    }

    [Fact]
    public void The_Stickiness_Expires()
    {
        // The window is a bet on the lag: too short and somebody sees their own edit
        // vanish, too long and the primary keeps the traffic. Either way it must expire,
        // or every session that ever wrote is pinned to the primary for ever.
        var (router, store, clock) = Build();
        router.Write("ada", "profile", "new");
        router.Read("ada", "profile");

        clock.Advance(Window);
        router.Read("ada", "profile");

        Assert.Equal(1, store.PrimaryReads);
        Assert.Equal(1, store.ReplicaReads);
    }

    [Fact]
    public void Adversarial_The_Replication_Lag_Is_Real()
    {
        // Stated as its own fact so the routing above cannot be dismissed as ceremony:
        // reading the replica immediately after a write genuinely returns the old value.
        var (router, store, clock) = Build();
        router.Write("ada", "profile", "old");
        clock.Advance(Lag);
        router.Write("ada", "profile", "new");

        Assert.Equal("old", store.ReadFromReplica("profile"));
        Assert.Equal("new", store.ReadFromPrimary("profile"));
    }

    [Fact]
    public void Adversarial_A_Different_Session_Really_Can_See_Stale_Data()
    {
        // The cost of the design, asserted rather than glossed over. Another user does see
        // the older value for as long as the lag lasts, and a system that cannot tolerate
        // that must not read replicas at all.
        var (router, _, clock) = Build();
        router.Write("ada", "profile", "old");
        clock.Advance(Lag);
        router.Write("ada", "profile", "new");

        Assert.Equal("new", router.Read("ada", "profile"));
        Assert.Equal("old", router.Read("grace", "profile"));
    }
}
