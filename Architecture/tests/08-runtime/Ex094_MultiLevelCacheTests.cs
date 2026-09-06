using FeWoLearning.Architecture.Exercises.Runtime.Ex094;
using FeWoLearning.Architecture.Tests.Harness;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex094_MultiLevelCacheTests
{
    private sealed class CountingLoader(string value = "loaded")
    {
        public int Calls { get; private set; }

        public string Load(string key)
        {
            Calls++;
            return $"{value}:{key}";
        }
    }

    private static (TwoLevelCache Cache, MemoryLevel L1, MemoryLevel L2) Build()
    {
        var l1 = new MemoryLevel("L1");
        var l2 = new MemoryLevel("L2");
        return (new TwoLevelCache(l1, l2), l1, l2);
    }

    [Fact]
    public void A_Miss_Loads_Once_And_Fills_Both_Levels()
    {
        var (cache, l1, l2) = Build();
        var loader = new CountingLoader();

        var value = cache.GetOrLoad("k", loader.Load);

        Assert.Equal("loaded:k", value);
        Assert.Equal(1, loader.Calls);
        Assert.True(l1.TryGet("k", out _));
        Assert.True(l2.TryGet("k", out _));
    }

    [Fact]
    public void Mechanism_An_L1_Hit_Does_Not_Touch_L2()
    {
        // Reading L2 anyway - to compare, to refresh, to be safe - means a local hit still
        // costs a network round trip, and L1 is buying nothing at all.
        var (cache, _, l2) = Build();
        var loader = new CountingLoader();
        cache.GetOrLoad("k", loader.Load);

        var readsAfterFill = l2.Reads;
        cache.GetOrLoad("k", loader.Load);

        Assert.Equal(readsAfterFill, l2.Reads);
        Assert.Equal(1, loader.Calls);
    }

    [Fact]
    public void Mechanism_An_L2_Hit_Is_Promoted_Into_L1()
    {
        // Without promotion a hot key is fetched from L2 on every single request, and the
        // local cache ends up holding only the keys nobody wants. This models a fresh
        // instance whose L1 is empty but whose shared cache is warm.
        var l1 = new MemoryLevel("L1");
        var l2 = new MemoryLevel("L2");
        l2.Set("k", "from-shared");
        var cache = new TwoLevelCache(l1, l2);
        var loader = new CountingLoader();

        Assert.Equal("from-shared", cache.GetOrLoad("k", loader.Load));

        var readsAfterPromotion = l2.Reads;
        Assert.Equal("from-shared", cache.GetOrLoad("k", loader.Load));

        Assert.Equal(readsAfterPromotion, l2.Reads);
        Assert.Equal(0, loader.Calls);
    }

    [Fact]
    public void Mechanism_Invalidation_Clears_Both_Levels()
    {
        // Clearing only the shared level leaves every instance serving its own stale copy
        // from L1 for as long as that entry lives - and it looks completely correct from
        // the instance that ran the invalidation, which is the one somebody is watching.
        var (cache, l1, l2) = Build();
        var loader = new CountingLoader();
        cache.GetOrLoad("k", loader.Load);

        cache.Invalidate("k");

        Assert.False(l1.TryGet("k", out _));
        Assert.False(l2.TryGet("k", out _));

        cache.GetOrLoad("k", loader.Load);
        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public void Adversarial_Invalidating_One_Key_Leaves_The_Others_Alone()
    {
        // Clearing the whole level is the easy way to pass the fact above, and it throws
        // away every unrelated hot key on every single invalidation - turning a correctness
        // fix into a periodic thundering herd.
        var (cache, _, _) = Build();
        var loader = new CountingLoader();
        cache.GetOrLoad("a", loader.Load);
        cache.GetOrLoad("b", loader.Load);

        cache.Invalidate("a");
        cache.GetOrLoad("b", loader.Load);

        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public void The_Loader_Runs_Once_Per_Miss_Not_Once_Per_Level()
    {
        var (cache, _, _) = Build();
        var loader = new CountingLoader();

        cache.GetOrLoad("a", loader.Load);
        cache.GetOrLoad("b", loader.Load);

        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public async Task Container_A_Second_Instance_Shares_L2_But_Not_L1()
    {
        // The shape this pattern actually has in production: two processes, two private L1
        // caches, one Redis between them. The exercise's own TwoLevelCache runs on both
        // sides. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new RedisBuilder("redis:7-alpine").Build();
        await container.StartAsync();

        using var redis = await ConnectionMultiplexer.ConnectAsync(container.GetConnectionString());
        var shared = new RedisLevel(redis.GetDatabase());

        var loader = new CountingLoader();
        var instanceA = new TwoLevelCache(new MemoryLevel("L1-a"), shared);
        var instanceB = new TwoLevelCache(new MemoryLevel("L1-b"), shared);

        Assert.Equal("loaded:k", instanceA.GetOrLoad("k", loader.Load));

        // B has a cold L1 and a warm L2, so it must not load again.
        Assert.Equal("loaded:k", instanceB.GetOrLoad("k", loader.Load));
        Assert.Equal(1, loader.Calls);

        // And an invalidation on B clears the shared copy, so A's next miss reloads.
        instanceB.Invalidate("k");
        Assert.False(shared.TryGet("k", out _));
    }

    private sealed class RedisLevel(IDatabase db) : ICacheLevel
    {
        public string Name => "L2-redis";

        public int Reads { get; private set; }

        public int Writes { get; private set; }

        public bool TryGet(string key, out string value)
        {
            Reads++;
            var raw = db.StringGet("cache:" + key);
            value = raw.IsNullOrEmpty ? "" : (string)raw!;
            return !raw.IsNullOrEmpty;
        }

        public void Set(string key, string value)
        {
            Writes++;
            db.StringSet("cache:" + key, value);
        }

        public void Remove(string key) => db.KeyDelete("cache:" + key);
    }
}
