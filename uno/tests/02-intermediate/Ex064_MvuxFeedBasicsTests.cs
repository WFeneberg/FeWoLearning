using FeWoLearning.Uno.Exercises.Intermediate;
using FeWoLearning.Uno.Support;
using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex064_MvuxFeedBasicsTests : UnoTestContext
{
    private static async Task<List<Ex064_Snapshot>> Snapshots(IFeed<int> feed, int count, Func<Task>? release = null) =>
        (await MvuxObserver.Collect(feed, count, release)).Select(Ex064_MvuxFeedBasics.Describe).ToList();

    [Fact]
    public async Task An_Immediate_Feed_Produces_One_Message_With_Data()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromResult(42));

        var snapshot = Assert.Single(await Snapshots(feed, 1));

        Assert.True(snapshot.HasData);
        Assert.Equal(42, snapshot.Data);
        Assert.False(snapshot.IsLoading);
        Assert.Null(snapshot.Error);
    }

    [Fact]
    public async Task A_Slow_Feed_Announces_Loading_First()
    {
        var gate = new TaskCompletionSource<int>();
        var feed = Ex064_MvuxFeedBasics.Create(_ => gate.Task);

        // Released only once the subscription is live, or the feed completes before anyone
        // is watching and the loading message is never produced.
        var snapshots = await Snapshots(feed, 2, () =>
        {
            gate.SetResult(7);
            return Task.CompletedTask;
        });

        // Two messages, and the first one carries no data at all - which is what lets a
        // view show a spinner without a separate IsLoading property.
        Assert.Equal(2, snapshots.Count);
        Assert.True(snapshots[0].IsLoading);
        Assert.False(snapshots[0].HasData);
    }

    [Fact]
    public async Task The_Second_Message_Carries_The_Value()
    {
        var gate = new TaskCompletionSource<int>();
        var feed = Ex064_MvuxFeedBasics.Create(_ => gate.Task);

        var snapshots = await Snapshots(feed, 2, () =>
        {
            gate.SetResult(7);
            return Task.CompletedTask;
        });

        Assert.Equal(7, snapshots[1].Data);
        Assert.False(snapshots[1].IsLoading);
    }

    [Fact]
    public async Task A_Failing_Feed_Carries_The_Error()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromException<int>(new InvalidOperationException("boom")));

        var snapshot = Assert.Single(await Snapshots(feed, 1));

        Assert.Equal("boom", snapshot.Error);
        Assert.False(snapshot.HasData);
    }

    [Fact]
    public async Task A_Failure_Is_Not_A_Deliberate_Absence()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromException<int>(new InvalidOperationException("boom")));

        var snapshot = Assert.Single(await Snapshots(feed, 1));

        // Undefined, not None. Collapsing the two into "null" loses the difference between
        // "no results" and "it broke".
        Assert.False(snapshot.IsEmpty);
    }

    [Fact]
    public async Task A_Failure_Is_Not_Reported_As_Loading()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromException<int>(new InvalidOperationException("boom")));

        var snapshot = Assert.Single(await Snapshots(feed, 1));

        Assert.False(snapshot.IsLoading);
    }

    [Fact]
    public async Task The_Current_Value_Can_Be_Read_Directly()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromResult(42));

        Assert.Equal(42, await Ex064_MvuxFeedBasics.CurrentValue(feed, CancellationToken.None));
    }

    [Fact]
    public async Task A_Failing_Feed_Has_No_Current_Value()
    {
        var feed = Ex064_MvuxFeedBasics.Create(_ => Task.FromException<int>(new InvalidOperationException("boom")));

        // The error reaches the caller of a direct read - which is exactly why a view binds
        // to the message rather than to a value.
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Ex064_MvuxFeedBasics.CurrentValue(feed, CancellationToken.None));
    }

    [Fact]
    public async Task The_Loader_Is_Given_A_Token()
    {
        var cancellable = false;
        var feed = Ex064_MvuxFeedBasics.Create(ct =>
        {
            cancellable = ct.CanBeCanceled;
            return Task.FromResult(1);
        });

        await Snapshots(feed, 1);

        Assert.True(cancellable);
    }
}
