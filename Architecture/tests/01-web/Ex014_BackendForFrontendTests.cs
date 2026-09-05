using FeWoLearning.Architecture.Exercises.Web.Ex014;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex014_BackendForFrontendTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

    private sealed class Fixed(string name, string value) : IUpstream
    {
        public string Name => name;

        public Task<string> FetchAsync(CancellationToken cancellationToken) =>
            Task.FromResult(value);
    }

    private sealed class Failing(string name, string message) : IUpstream
    {
        public string Name => name;

        public Task<string> FetchAsync(CancellationToken cancellationToken) =>
            Task.FromException<string>(new InvalidOperationException(message));
    }

    /// <summary>
    /// Blocks until every participant has arrived. A sequential aggregator never gets
    /// past the first one, so the difference shows up as "did not finish" rather than
    /// as "finished slowly" - no stopwatch, no flakiness on a loaded machine.
    /// </summary>
    private sealed class Rendezvous(string name, CountdownEvent arrivals, TaskCompletionSource gate) : IUpstream
    {
        public string Name => name;

        public async Task<string> FetchAsync(CancellationToken cancellationToken)
        {
            arrivals.Signal();
            await gate.Task;
            return "value:" + name;
        }
    }

    [Fact]
    public async Task Use_All_Upstreams_Succeeding_Produces_One_Entry_Each()
    {
        var result = await Ex014_BackendForFrontend.AggregateAsync(
            [new Fixed("profile", "p"), new Fixed("orders", "o"), new Fixed("offers", "f")]);

        Assert.Equal(3, result.Data.Count);
        Assert.Equal("p", result.Data["profile"]);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Adversarial_One_Failing_Upstream_Does_Not_Fail_The_Aggregate()
    {
        // A page with three panels renders two of them when the third service is down.
        // Letting the exception escape - or catching around Task.WhenAll, which
        // surfaces only the first one - loses that.
        var result = await Ex014_BackendForFrontend.AggregateAsync(
            [new Fixed("profile", "p"), new Failing("orders", "orders is down"), new Fixed("offers", "f")]);

        Assert.Equal(["offers", "profile"], result.Data.Keys.OrderBy(k => k));
        Assert.Equal("orders is down", result.Errors["orders"]);
    }

    [Fact]
    public async Task Two_Failing_Upstreams_Are_Both_Reported()
    {
        // Catching a single exception around Task.WhenAll passes the fact above and
        // silently drops the second failure - the caller is told one panel is broken
        // when two are.
        var result = await Ex014_BackendForFrontend.AggregateAsync(
            [new Failing("orders", "a"), new Failing("offers", "b"), new Fixed("profile", "p")]);

        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("a", result.Errors["orders"]);
        Assert.Equal("b", result.Errors["offers"]);
    }

    [Fact]
    public async Task Mechanism_Every_Upstream_Is_Started_Before_Any_Is_Awaited()
    {
        // The parallelism fact, made deterministic: nobody returns until everybody has
        // arrived. An implementation that awaits inside its loop deadlocks here and
        // fails on the timeout instead of passing a stopwatch by luck.
        using var arrivals = new CountdownEvent(3);
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var aggregating = Ex014_BackendForFrontend.AggregateAsync(
        [
            new Rendezvous("a", arrivals, gate),
            new Rendezvous("b", arrivals, gate),
            new Rendezvous("c", arrivals, gate),
        ]);

        Assert.True(arrivals.Wait(Patience), "not every upstream was started before the first was awaited");
        gate.SetResult();

        var result = await aggregating.WaitAsync(Patience);

        Assert.Equal(3, result.Data.Count);
        Assert.Equal("value:b", result.Data["b"]);
    }

    [Fact]
    public async Task An_Empty_Upstream_List_Aggregates_To_Nothing()
    {
        var result = await Ex014_BackendForFrontend.AggregateAsync([]);

        Assert.Empty(result.Data);
        Assert.Empty(result.Errors);
    }
}
