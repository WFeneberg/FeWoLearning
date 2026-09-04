using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex064_AsyncLoadOnDemandTests : UnoTestContext
{
    [Fact]
    public async Task A_Successful_Load_Ends_Loaded()
    {
        var loader = new Ex064_AsyncLoadOnDemand((query, _) => Task.FromResult($"data for {query}"));

        // Nothing has been asked for yet.
        Assert.Equal(Ex064_LoadState.Idle, loader.State);
        Assert.Null(loader.Data);

        await loader.LoadAsync("a");

        Assert.Equal(Ex064_LoadState.Loaded, loader.State);
        Assert.Equal("data for a", loader.Data);
        Assert.Null(loader.Error);
    }

    [Fact]
    public async Task A_Failed_Load_Ends_Failed()
    {
        var loader = new Ex064_AsyncLoadOnDemand((_, _) => Task.FromException<string>(new InvalidOperationException("boom")));

        await loader.LoadAsync("a");

        Assert.Equal(Ex064_LoadState.Failed, loader.State);
        Assert.Equal("boom", loader.Error);
    }

    [Fact]
    public async Task Loading_Is_Announced_Before_The_Result()
    {
        var gate = new TaskCompletionSource<string>();
        var loader = new Ex064_AsyncLoadOnDemand((_, _) => gate.Task);

        var loading = loader.LoadAsync("a");
        Assert.Equal(Ex064_LoadState.Loading, loader.State);

        gate.SetResult("done");
        await loading;

        Assert.Equal(Ex064_LoadState.Loaded, loader.State);
    }

    [Fact]
    public async Task A_Second_Request_Cancels_The_First()
    {
        var cancelled = false;
        var first = new TaskCompletionSource<string>();
        var loader = new Ex064_AsyncLoadOnDemand(async (query, ct) =>
        {
            if (query == "slow")
            {
                ct.Register(() => cancelled = true);
                return await first.Task;
            }

            return "fast";
        });

        var slow = loader.LoadAsync("slow");
        await loader.LoadAsync("fast");

        Assert.True(cancelled);

        first.SetResult("slow answer");
        await slow;
    }

    [Fact]
    public async Task The_Superseded_Answer_Is_Discarded()
    {
        var first = new TaskCompletionSource<string>();
        var loader = new Ex064_AsyncLoadOnDemand((query, _) =>
            query == "slow" ? first.Task : Task.FromResult("fast"));

        var slow = loader.LoadAsync("slow");
        await loader.LoadAsync("fast");
        first.SetResult("slow answer");
        await slow;

        // The bug that outlives every framework: A, then B, B answers first, then A
        // overwrites the screen with stale data. Cancelling is half the fix; ignoring the
        // answer of a cancelled request is the other half.
        Assert.Equal("fast", loader.Data);
        Assert.Equal(Ex064_LoadState.Loaded, loader.State);
    }

    [Fact]
    public async Task A_Cancelled_Load_Is_Not_A_Failure()
    {
        var first = new TaskCompletionSource<string>();
        var loader = new Ex064_AsyncLoadOnDemand(async (query, ct) =>
            query == "slow" ? await first.Task.WaitAsync(ct) : "fast");

        var slow = loader.LoadAsync("slow");
        await loader.LoadAsync("fast");
        await slow;

        // OperationCanceledException means "nobody wants this any more", not "it broke".
        Assert.Equal(Ex064_LoadState.Loaded, loader.State);
        Assert.Null(loader.Error);
    }

    [Fact]
    public async Task A_Later_Success_Clears_An_Earlier_Error()
    {
        var fail = true;
        var loader = new Ex064_AsyncLoadOnDemand((_, _) =>
            fail ? Task.FromException<string>(new InvalidOperationException("boom")) : Task.FromResult("ok"));

        await loader.LoadAsync("a");
        fail = false;
        await loader.LoadAsync("a");

        Assert.Equal(Ex064_LoadState.Loaded, loader.State);
        Assert.Null(loader.Error);
    }

    [Fact]
    public async Task The_State_Changes_Are_Announced()
    {
        var loader = new Ex064_AsyncLoadOnDemand((_, _) => Task.FromResult("x"));
        var names = new List<string?>();
        loader.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        await loader.LoadAsync("a");

        Assert.Contains(nameof(Ex064_AsyncLoadOnDemand.State), names);
        Assert.Contains(nameof(Ex064_AsyncLoadOnDemand.Data), names);
    }
}
