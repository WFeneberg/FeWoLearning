using FeWoLearning.Architecture.Exercises.ServicesData.Ex035;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex035_WriteThroughWriteBehindTests
{
    [Fact]
    public void Write_Through_Reaches_The_Store_Immediately()
    {
        var store = new BackingStore();
        var cache = new WriteThroughCache(store);

        cache.Write("k", "new");

        Assert.Equal("new", store.Read("k"));
        Assert.Equal(1, store.Writes);
    }

    [Fact]
    public void Mechanism_Write_Behind_Leaves_The_Store_Holding_The_Old_Value()
    {
        // Asserted from the STORE, never from the cache. "Write-behind" that writes
        // through immediately returns identical values from every read, passes any
        // assertion made through the cache, and has quietly given up the only benefit
        // the policy has - while the design document still calls it deferred.
        var store = new BackingStore();
        store.Write("k", "old");
        var cache = new WriteBehindCache(store);

        cache.Write("k", "new");

        Assert.Equal("old", store.Read("k"));
        Assert.Equal("new", cache.Read("k"));
    }

    [Fact]
    public void Flushing_Closes_The_Gap()
    {
        var store = new BackingStore();
        store.Write("k", "old");
        var cache = new WriteBehindCache(store);
        cache.Write("k", "new");

        cache.Flush();

        Assert.Equal("new", store.Read("k"));
    }

    [Fact]
    public void Mechanism_Repeated_Write_Behind_Writes_To_One_Key_Cost_One_Store_Write()
    {
        // The whole reason to accept the loss window. An implementation that queues each
        // write instead of tracking dirty KEYS passes every fact above and does exactly
        // as much store work as write-through, having taken on the risk for nothing.
        var store = new BackingStore();
        var cache = new WriteBehindCache(store);

        cache.Write("k", "one");
        cache.Write("k", "two");
        cache.Write("k", "three");
        cache.Flush();

        Assert.Equal(1, store.Writes);
        Assert.Equal("three", store.Read("k"));
    }

    [Fact]
    public void Write_Through_Does_Not_Coalesce_Which_Is_The_Trade()
    {
        // The other side of the same coin, asserted so the two policies cannot converge
        // on one implementation.
        var store = new BackingStore();
        var cache = new WriteThroughCache(store);

        cache.Write("k", "one");
        cache.Write("k", "two");
        cache.Write("k", "three");

        Assert.Equal(3, store.Writes);
    }

    [Fact]
    public void Flushing_Twice_Does_Not_Write_Twice()
    {
        var store = new BackingStore();
        var cache = new WriteBehindCache(store);
        cache.Write("k", "one");

        cache.Flush();
        cache.Flush();

        Assert.Equal(1, store.Writes);
    }

    [Fact]
    public void Both_Caches_Read_Through_To_The_Store_For_Keys_They_Have_Never_Seen()
    {
        var store = new BackingStore();
        store.Write("k", "from store");

        Assert.Equal("from store", new WriteThroughCache(store).Read("k"));
        Assert.Equal("from store", new WriteBehindCache(store).Read("k"));
    }
}
