using FeWoLearning.Architecture.Exercises.Scale.Ex061;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex061_BulkheadIsolationTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    private static readonly Dictionary<string, int> Capacities = new()
    {
        ["payments"] = 2,
        ["search"] = 2,
    };

    /// <summary>Same fail-fast gate wait the concurrency rows elsewhere use.</summary>
    private static void WaitForArrival(CountdownEvent arrived, params Task[] racers)
    {
        var deadline = DateTime.UtcNow + Patience;

        while (!arrived.Wait(TimeSpan.FromMilliseconds(25)))
        {
            foreach (var racer in racers)
                if (racer.IsFaulted)
                    racer.GetAwaiter().GetResult();

            Assert.True(DateTime.UtcNow < deadline, "the callers never reached the gate");
        }
    }

    [Fact]
    public async Task Work_Runs_And_Returns()
    {
        var bulkhead = new Bulkhead(Capacities);

        Assert.Equal("ok", await bulkhead.ExecuteAsync("payments", () => Task.FromResult("ok")));
    }

    [Fact]
    public async Task Mechanism_A_Full_Partition_Rejects_Without_Running_The_Work()
    {
        // Rejected NOW, not queued. Queueing turns the bulkhead into a buffer, and the
        // queue is exactly where the latency the pattern exists to contain reappears -
        // a caller that is going to fail should fail cheaply.
        var bulkhead = new Bulkhead(Capacities);
        using var arrived = new CountdownEvent(2);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var extraWorkRan = 0;

        var holders = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => bulkhead.ExecuteAsync("payments", async () =>
            {
                arrived.Signal();
                await release.Task;
                return "held";
            })))
            .ToArray();

        WaitForArrival(arrived, holders);

        // WaitAsync for the same reason as Ex062: an implementation that queues rather
        // than rejecting must FAIL this fact, not hang on it.
        var rejection = await Assert.ThrowsAsync<BulkheadRejectedException>(
            () => bulkhead.ExecuteAsync("payments", () =>
            {
                Interlocked.Increment(ref extraWorkRan);
                return Task.FromResult("should not run");
            }).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal("payments", rejection.Partition);
        Assert.Equal(0, extraWorkRan);

        release.SetResult();
        await Task.WhenAll(holders).WaitAsync(Patience);
    }

    [Fact]
    public async Task Mechanism_Saturating_One_Partition_Leaves_The_Other_Available()
    {
        // The pattern itself. One shared semaphore satisfies every fact above and is not
        // a bulkhead: without this, a payment provider that starts taking thirty seconds
        // instead of thirty milliseconds consumes every slot in the process and the site
        // stops serving its home page. The failure reads as "everything is down".
        var bulkhead = new Bulkhead(Capacities);
        using var arrived = new CountdownEvent(2);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var holders = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => bulkhead.ExecuteAsync("payments", async () =>
            {
                arrived.Signal();
                await release.Task;
                return "held";
            })))
            .ToArray();

        WaitForArrival(arrived, holders);

        Assert.Equal(2, bulkhead.InFlight("payments"));
        Assert.Equal("search still works",
            await bulkhead.ExecuteAsync("search", () => Task.FromResult("search still works")).WaitAsync(Patience));

        release.SetResult();
        await Task.WhenAll(holders).WaitAsync(Patience);
    }

    [Fact]
    public async Task A_Slot_Comes_Back_When_The_Work_Finishes()
    {
        var bulkhead = new Bulkhead(Capacities);

        for (var i = 0; i < 10; i++)
            await bulkhead.ExecuteAsync("payments", () => Task.FromResult(i));

        Assert.Equal(0, bulkhead.InFlight("payments"));
    }

    [Fact]
    public async Task Adversarial_A_Slot_Comes_Back_When_The_Work_Throws()
    {
        // Releasing after the await rather than in a finally leaks a slot on every
        // failure, so the partition closes permanently the first time the dependency
        // misbehaves - which is precisely when it is needed.
        var bulkhead = new Bulkhead(Capacities);

        for (var i = 0; i < 5; i++)
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                bulkhead.ExecuteAsync<int>("payments", () => throw new InvalidOperationException("upstream")));

        Assert.Equal(0, bulkhead.InFlight("payments"));
        Assert.Equal(1, await bulkhead.ExecuteAsync("payments", () => Task.FromResult(1)).WaitAsync(Patience));
    }

    [Fact]
    public async Task A_Partition_With_No_Declared_Capacity_Is_Unlimited()
    {
        var bulkhead = new Bulkhead(Capacities);

        Assert.Equal("ok", await bulkhead.ExecuteAsync("undeclared", () => Task.FromResult("ok")));
    }
}
