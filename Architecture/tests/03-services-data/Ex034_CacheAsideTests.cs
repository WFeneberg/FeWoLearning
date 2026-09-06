using FeWoLearning.Architecture.Exercises.ServicesData.Ex034;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex034_CacheAsideTests
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    private sealed class CountingLoader
    {
        public int Calls { get; private set; }

        public string Load(string key)
        {
            Calls++;
            return $"{key}:v{Calls}";
        }
    }

    private static (CacheAside<string, string> Cache, ManualClock Clock, CountingLoader Loader) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new CacheAside<string, string>(clock, Ttl), clock, new CountingLoader());
    }

    [Fact]
    public void Mechanism_A_Second_Read_Of_The_Same_Key_Does_Not_Call_The_Loader()
    {
        // The count is the only thing separating a cache from no cache. "Returns the
        // right value" is satisfied perfectly by calling the loader every single time -
        // which is to say, by deleting the cache.
        var (cache, _, loader) = Build();

        var first = cache.GetOrLoad("a", loader.Load);
        var second = cache.GetOrLoad("a", loader.Load);

        Assert.Equal(1, loader.Calls);
        Assert.Equal("a:v1", first);
        Assert.Equal("a:v1", second);
    }

    [Fact]
    public void Entries_Are_Per_Key()
    {
        // Catches a one-slot cache, which passes the fact above and then thrashes on
        // any real workload.
        var (cache, _, loader) = Build();

        cache.GetOrLoad("a", loader.Load);
        cache.GetOrLoad("b", loader.Load);
        cache.GetOrLoad("a", loader.Load);

        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public void Within_The_Ttl_The_Entry_Is_Still_Live()
    {
        var (cache, clock, loader) = Build();
        cache.GetOrLoad("a", loader.Load);

        clock.Advance(Ttl - TimeSpan.FromSeconds(1));
        cache.GetOrLoad("a", loader.Load);

        Assert.Equal(1, loader.Calls);
    }

    [Fact]
    public void Mechanism_Past_The_Ttl_The_Value_Is_Loaded_Again()
    {
        // A cache with no expiry passes every fact above and serves a value from last
        // Tuesday. The clock is advanced by hand, so the assertion is about the TTL and
        // not about how fast the machine is.
        var (cache, clock, loader) = Build();
        cache.GetOrLoad("a", loader.Load);

        clock.Advance(Ttl);

        Assert.Equal("a:v2", cache.GetOrLoad("a", loader.Load));
        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public void Invalidating_Forces_The_Next_Read_To_Reload()
    {
        var (cache, _, loader) = Build();
        cache.GetOrLoad("a", loader.Load);

        cache.Invalidate("a");

        Assert.Equal("a:v2", cache.GetOrLoad("a", loader.Load));
        Assert.Equal(2, loader.Calls);
    }

    [Fact]
    public void Invalidating_An_Unknown_Key_Is_Harmless()
    {
        var (cache, _, loader) = Build();
        cache.GetOrLoad("a", loader.Load);

        cache.Invalidate("never-cached");

        Assert.Equal(1, loader.Calls);
    }
}
