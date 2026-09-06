using FeWoLearning.Architecture.Exercises.ServicesData.Ex037;
using FeWoLearning.Architecture.Exercises.Support;
using FeWoLearning.Architecture.Tests.Harness;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex037_DistributedLockTests
{
    private static readonly TimeSpan Duration = TimeSpan.FromSeconds(30);

    private static (LeaseManager Manager, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new LeaseManager(clock), clock);
    }

    [Fact]
    public void A_Free_Resource_Is_Granted_And_Then_Held()
    {
        var (manager, _) = Build();

        Assert.NotNull(manager.TryAcquire("report", "A", Duration));
        Assert.Null(manager.TryAcquire("report", "B", Duration));
    }

    [Fact]
    public void Mechanism_An_Expired_Lease_Is_Granted_To_Somebody_Else()
    {
        // A lease that only checks "is it held" turns a crashed holder into a
        // permanently unavailable resource - the exact failure a lease exists to avoid.
        var (manager, clock) = Build();
        manager.TryAcquire("report", "A", Duration);

        clock.Advance(Duration);

        Assert.NotNull(manager.TryAcquire("report", "B", Duration));
    }

    [Fact]
    public void The_Owner_May_Release_Early_And_Nobody_Else_May()
    {
        var (manager, _) = Build();
        manager.TryAcquire("report", "A", Duration);

        Assert.False(manager.Release("report", "B"));
        Assert.Null(manager.TryAcquire("report", "B", Duration));

        Assert.True(manager.Release("report", "A"));
        Assert.NotNull(manager.TryAcquire("report", "B", Duration));
    }

    [Fact]
    public void Fencing_Tokens_Only_Ever_Increase()
    {
        // A counter that restarts when a lease expires lets a stale writer look current
        // again, which is precisely what the token exists to prevent.
        var (manager, clock) = Build();

        var first = manager.TryAcquire("report", "A", Duration)!;
        clock.Advance(Duration);
        var second = manager.TryAcquire("report", "B", Duration)!;

        Assert.True(second.FencingToken > first.FencingToken);
    }

    [Fact]
    public void Mechanism_A_Stale_Holder_Cannot_Write_After_Its_Lease_Moved_On()
    {
        // The exercise. Everything above passes for a lease with expiry and no fencing
        // token - and that design still corrupts data, because a lease cannot stop the
        // PREVIOUS holder. A was paused (GC, stalled disk, VM migration) and wakes up
        // believing it still holds a lock that expired minutes ago. Nothing has told it
        // otherwise and nothing can; the resource has to notice.
        var (manager, clock) = Build();
        var resource = new FencedResource();

        var a = manager.TryAcquire("report", "A", Duration)!;

        clock.Advance(Duration);
        var b = manager.TryAcquire("report", "B", Duration)!;

        Assert.True(resource.TryWrite(b.FencingToken, "written by B"));

        // A wakes up here, still holding what it thinks is a valid lease.
        Assert.False(resource.TryWrite(a.FencingToken, "written by A"));
        Assert.Equal("written by B", resource.Value);
    }

    [Fact]
    public void The_Current_Holder_May_Write_Repeatedly()
    {
        // Pairs with the fact above: "reject older tokens" must not become "reject the
        // second write from the same holder", which would make the lease useless.
        var (manager, _) = Build();
        var resource = new FencedResource();
        var lease = manager.TryAcquire("report", "A", Duration)!;

        Assert.True(resource.TryWrite(lease.FencingToken, "one"));
        Assert.True(resource.TryWrite(lease.FencingToken, "two"));
        Assert.Equal("two", resource.Value);
    }

    [Fact]
    public async Task Container_A_Real_Redis_Token_Source_Still_Fences_The_Stale_Holder()
    {
        // Drives the EXERCISE'S FencedResource with tokens from a real distributed
        // counter, rather than merely demonstrating that Redis has SET NX. Redis
        // supplies the monotonic token and the atomic grant; the exercise supplies the
        // fencing, which is the half that actually protects the data. Skipped unless
        // -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new RedisBuilder("redis:7-alpine").Build();
        await container.StartAsync();

        using var redis = await ConnectionMultiplexer.ConnectAsync(container.GetConnectionString());
        var db = redis.GetDatabase();
        var resource = new FencedResource();

        // A takes the lease with a very short TTL and gets token 1.
        Assert.True(await db.StringSetAsync("lease:report", "A", TimeSpan.FromMilliseconds(150), When.NotExists));
        var tokenA = await db.StringIncrementAsync("fence:report");

        // A second caller is refused while the lease is live - the primitive is atomic.
        Assert.False(await db.StringSetAsync("lease:report", "B", TimeSpan.FromSeconds(30), When.NotExists));

        // The lease expires on its own. Nobody tells A.
        while (await db.KeyExistsAsync("lease:report"))
            await Task.Delay(25);

        Assert.True(await db.StringSetAsync("lease:report", "B", TimeSpan.FromSeconds(30), When.NotExists));
        var tokenB = await db.StringIncrementAsync("fence:report");

        Assert.True(resource.TryWrite(tokenB, "written by B"));
        Assert.False(resource.TryWrite(tokenA, "written by A"));
        Assert.Equal("written by B", resource.Value);
    }
}
