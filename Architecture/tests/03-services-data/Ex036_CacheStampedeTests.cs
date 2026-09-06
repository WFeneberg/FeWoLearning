using FeWoLearning.Architecture.Exercises.ServicesData.Ex036;
using FeWoLearning.Architecture.Tests.Harness;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex036_CacheStampedeTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Waits for the rendezvous gate, but gives up the moment one of the racers has
    /// already failed. Without this, an unimplemented stub - which throws instantly and
    /// therefore never reaches the gate - makes every gated fact sit out the full
    /// timeout, and the red run of this one exercise took 45 seconds.
    /// </summary>
    private static void WaitForArrival(CountdownEvent arrived, params Task[] racers)
    {
        var deadline = DateTime.UtcNow + Patience;

        while (!arrived.Wait(TimeSpan.FromMilliseconds(25)))
        {
            foreach (var racer in racers)
                if (racer.IsFaulted)
                    racer.GetAwaiter().GetResult(); // rethrows what the racer threw

            Assert.True(DateTime.UtcNow < deadline, "the loader never reached the gate");
        }
    }

    [Fact]
    public async Task A_Warm_Key_Does_Not_Load_Again()
    {
        var cache = new SingleFlightCache<string, string>();
        var calls = 0;

        await cache.GetOrLoadAsync("a", _ => { Interlocked.Increment(ref calls); return Task.FromResult("v"); });
        await cache.GetOrLoadAsync("a", _ => { Interlocked.Increment(ref calls); return Task.FromResult("v"); });

        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task Mechanism_Many_Callers_On_A_Cold_Key_Cause_Exactly_One_Load()
    {
        // The row's whole point, made deterministic. The loader blocks until every
        // caller has arrived, so plain cache-aside - which finds an empty slot and
        // starts its own load - registers eight invocations rather than being merely
        // slower. No stopwatch, nothing to be flaky about on a loaded machine.
        const int callers = 8;
        var cache = new SingleFlightCache<string, string>();
        var calls = 0;
        using var arrived = new CountdownEvent(1);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<string> Load(string key)
        {
            Interlocked.Increment(ref calls);
            arrived.Signal();
            await release.Task;
            return key + ":loaded";
        }

        var readers = Enumerable.Range(0, callers)
            .Select(_ => Task.Run(() => cache.GetOrLoadAsync("hot", Load)))
            .ToArray();

        WaitForArrival(arrived, readers);
        release.SetResult();

        var results = await Task.WhenAll(readers).WaitAsync(Patience);

        Assert.Equal(1, calls);
        Assert.All(results, r => Assert.Equal("hot:loaded", r));
    }

    [Fact]
    public async Task Mechanism_Two_Different_Cold_Keys_Load_Concurrently()
    {
        // Catches the single global lock, which produces exactly one load per key and
        // serialises every unrelated key behind it - turning the cache into the
        // bottleneck it was installed to remove. Both loads must be in flight at once,
        // so a serialising implementation deadlocks here.
        var cache = new SingleFlightCache<string, string>();
        using var bothStarted = new CountdownEvent(2);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<string> Load(string key)
        {
            bothStarted.Signal();
            await release.Task;
            return key;
        }

        var a = Task.Run(() => cache.GetOrLoadAsync("a", Load));
        var b = Task.Run(() => cache.GetOrLoadAsync("b", Load));

        WaitForArrival(bothStarted, a, b);
        release.SetResult();

        Assert.Equal(["a", "b"], await Task.WhenAll(a, b).WaitAsync(Patience));
    }

    [Fact]
    public async Task Adversarial_A_Failed_Load_Does_Not_Poison_The_Key()
    {
        // Caching the task unconditionally is the natural implementation and it passes
        // every fact above. It also caches the FAILURE, so one transient error takes the
        // key out for the lifetime of the process.
        var cache = new SingleFlightCache<string, string>();
        var attempt = 0;

        Task<string> Load(string key) =>
            Interlocked.Increment(ref attempt) == 1
                ? Task.FromException<string>(new InvalidOperationException("upstream is down"))
                : Task.FromResult("recovered");

        await Assert.ThrowsAsync<InvalidOperationException>(() => cache.GetOrLoadAsync("a", Load));

        Assert.Equal("recovered", await cache.GetOrLoadAsync("a", Load));
    }

    [Fact]
    public async Task Every_Waiter_On_A_Failed_Load_Sees_The_Failure()
    {
        // Pairs with the fact above: "do not cache failures" must not become "the second
        // caller silently gets a default value".
        var cache = new SingleFlightCache<string, string>();
        using var arrived = new CountdownEvent(1);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<string> Load(string key)
        {
            arrived.Signal();
            await release.Task;
            throw new InvalidOperationException("upstream is down");
        }

        var first = Task.Run(() => cache.GetOrLoadAsync("a", Load));
        WaitForArrival(arrived, first);
        var second = Task.Run(() => cache.GetOrLoadAsync("a", Load));

        release.SetResult();

        await Assert.ThrowsAsync<InvalidOperationException>(() => first.WaitAsync(Patience));
        await Assert.ThrowsAsync<InvalidOperationException>(() => second.WaitAsync(Patience));
    }

    [Fact]
    public async Task Container_A_Real_Redis_Round_Trip_Still_Loads_Once()
    {
        // The in-process facts prove the single-flight logic. This one proves it still
        // holds when the "load" is a real network round trip to a real server, which is
        // where the stampede actually hurts. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var redisContainer = new RedisBuilder("redis:7-alpine").Build();
        await redisContainer.StartAsync();

        using var redis = await ConnectionMultiplexer.ConnectAsync(redisContainer.GetConnectionString());
        var db = redis.GetDatabase();
        await db.StringSetAsync("hot", "from-redis");

        const int callers = 8;
        var cache = new SingleFlightCache<string, string>();
        var calls = 0;

        async Task<string> Load(string key)
        {
            Interlocked.Increment(ref calls);
            return (await db.StringGetAsync(key))!;
        }

        var results = await Task.WhenAll(
            Enumerable.Range(0, callers).Select(_ => Task.Run(() => cache.GetOrLoadAsync("hot", Load))));

        Assert.Equal(1, calls);
        Assert.All(results, r => Assert.Equal("from-redis", r));
    }
}
