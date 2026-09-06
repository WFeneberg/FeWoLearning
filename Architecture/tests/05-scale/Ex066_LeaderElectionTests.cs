using FeWoLearning.Architecture.Exercises.Scale.Ex066;
using FeWoLearning.Architecture.Exercises.Support;
using FeWoLearning.Architecture.Tests.Harness;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex066_LeaderElectionTests
{
    private const string Resource = "nightly-report";
    private static readonly TimeSpan Lease = TimeSpan.FromSeconds(30);

    private static (ManualClock Clock, ILeaseStore Store, LeaderElection A, LeaderElection B) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var store = new InMemoryLeaseStore();
        return (clock,
                store,
                new LeaderElection(clock, store, Resource, "node-a", Lease),
                new LeaderElection(clock, store, Resource, "node-b", Lease));
    }

    [Fact]
    public void The_First_Node_To_Heartbeat_Leads_And_The_Second_Does_Not()
    {
        var (_, _, a, b) = Build();

        Assert.True(a.Heartbeat());
        Assert.False(b.Heartbeat());

        Assert.True(a.IsLeader);
        Assert.False(b.IsLeader);
    }

    [Fact]
    public void Adversarial_Renewing_Does_Not_Cost_The_Leader_Its_Leadership()
    {
        // A store that refuses to renew a live lease makes every heartbeat a re-election,
        // and leadership flaps between nodes for no reason at all. It is an easy mistake:
        // "the lease is held, so refuse" is correct for everybody except the holder.
        var (clock, _, a, b) = Build();
        a.Heartbeat();

        for (var i = 0; i < 5; i++)
        {
            clock.Advance(Lease / 3);
            Assert.True(a.Heartbeat());
            Assert.False(b.Heartbeat());
        }

        Assert.True(a.IsLeader);
    }

    [Fact]
    public void Mechanism_Leadership_Expires_On_Its_Own_Without_Anybody_Asking()
    {
        // The fact this exercise exists for. A flag set when the last heartbeat succeeded
        // describes the past: node A stops heartbeating - because it is paused, or
        // partitioned, or its process is swapping - and goes on believing it is leader
        // until it next asks. "Until it next asks" is precisely the window in which the
        // thing that must happen only once happens twice.
        var (clock, _, a, _) = Build();
        a.Heartbeat();
        Assert.True(a.IsLeader);

        clock.Advance(Lease);

        // No call to Heartbeat, nobody told it anything - and it must already know.
        Assert.False(a.IsLeader);
    }

    [Fact]
    public void An_Expired_Lease_Is_Taken_By_Somebody_Else()
    {
        var (clock, _, a, b) = Build();
        a.Heartbeat();

        clock.Advance(Lease);

        Assert.True(b.Heartbeat());
        Assert.True(b.IsLeader);
        Assert.False(a.IsLeader);
    }

    [Fact]
    public void A_Displaced_Leader_Is_Refused_When_It_Comes_Back()
    {
        // The other half of takeover: the old leader must be told no, not silently
        // allowed to reclaim a lease somebody else is now holding.
        var (clock, _, a, b) = Build();
        a.Heartbeat();
        clock.Advance(Lease);
        b.Heartbeat();

        Assert.False(a.Heartbeat());
        Assert.False(a.IsLeader);
        Assert.True(b.IsLeader);
    }

    [Fact]
    public void Resigning_Frees_The_Lease_Immediately()
    {
        // Without this, a graceful shutdown costs the cluster a full lease duration of
        // having no leader at all - for no reason, since the node knew it was leaving.
        var (_, _, a, b) = Build();
        a.Heartbeat();

        a.Resign();

        Assert.False(a.IsLeader);
        Assert.True(b.Heartbeat());
    }

    [Fact]
    public void Resigning_A_Lease_Somebody_Else_Holds_Does_Nothing()
    {
        var (_, _, a, b) = Build();
        a.Heartbeat();

        b.Resign();

        Assert.True(a.IsLeader);
    }

    [Fact]
    public async Task Container_The_Same_Election_Runs_Against_Real_Redis()
    {
        // The store is the part that has to be atomic, and in production it is a network
        // service several nodes race on. This runs the exercise's own LeaderElection over
        // a Redis-backed store through the same interface. Skipped unless
        // -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new RedisBuilder("redis:7-alpine").Build();
        await container.StartAsync();

        using var redis = await ConnectionMultiplexer.ConnectAsync(container.GetConnectionString());
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var store = new RedisLeaseStore(redis.GetDatabase());

        var a = new LeaderElection(clock, store, Resource, "node-a", Lease);
        var b = new LeaderElection(clock, store, Resource, "node-b", Lease);

        Assert.True(a.Heartbeat());
        Assert.False(b.Heartbeat());

        clock.Advance(Lease / 2);
        Assert.True(a.Heartbeat());   // renewal, not re-election
        Assert.False(b.Heartbeat());

        clock.Advance(Lease);
        Assert.False(a.IsLeader);     // expired against the clock, with nobody asking
        Assert.True(b.Heartbeat());
        Assert.False(a.Heartbeat());
    }

    /// <summary>
    /// Redis-backed, but the expiry is judged against the exercise's clock rather than
    /// Redis's own TTL - the tests advance a ManualClock, which a real TTL would not
    /// follow. The atomicity being exercised is the compare-and-set on the holder.
    /// </summary>
    private sealed class RedisLeaseStore(IDatabase db) : ILeaseStore
    {
        private static string Key(string resource) => "lease:" + resource;

        public bool TryAcquireOrRenew(string resource, string nodeId, DateTimeOffset now, TimeSpan duration)
        {
            if (HolderOf(resource, now) is { } holder && holder != nodeId)
                return false;

            db.StringSet(Key(resource), $"{nodeId}|{(now + duration).ToUnixTimeMilliseconds()}");
            return true;
        }

        public void Release(string resource, string nodeId)
        {
            var raw = db.StringGet(Key(resource));
            if (!raw.IsNullOrEmpty && ((string)raw!).Split('|')[0] == nodeId)
                db.KeyDelete(Key(resource));
        }

        public string? HolderOf(string resource, DateTimeOffset now)
        {
            var raw = db.StringGet(Key(resource));
            if (raw.IsNullOrEmpty)
                return null;

            var parts = ((string)raw!).Split('|');
            var expiresAt = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parts[1]));
            return expiresAt > now ? parts[0] : null;
        }
    }
}
