using FeWoLearning.Uno.Exercises.Expert;
using FeWoLearning.Uno.Support;
using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex093_MvuxFeedCompositionTests : UnoTestContext
{
    // Feed.Async takes an AsyncFunc<T> - a ValueTask, not a Task.
    private static IFeed<int> Number(int value) => Feed.Async(_ => ValueTask.FromResult(value));

    private static IFeed<string> Text(string value) => Feed.Async(_ => ValueTask.FromResult(value));

    private static IFeed<int> Failing() =>
        Feed.Async<int>(_ => ValueTask.FromException<int>(new InvalidOperationException("boom")));

    private static async Task<Ex093_Outcome> Outcome(IFeed<string> feed) =>
        Ex093_MvuxFeedComposition.Describe(Assert.Single(await MvuxObserver.Collect(feed, 1)));

    [Fact]
    public async Task A_Projection_Maps_The_Data()
    {
        var outcome = await Outcome(Ex093_MvuxFeedComposition.Format(Number(21)));

        Assert.True(outcome.HasValue);
        Assert.Equal("#21", outcome.Value);
    }

    [Fact]
    public async Task An_Error_Travels_Through_A_Projection()
    {
        var outcome = await Outcome(Ex093_MvuxFeedComposition.Format(Failing()));

        Assert.Equal("boom", outcome.Error);
        Assert.False(outcome.HasValue);
    }

    [Fact]
    public async Task The_Projection_Is_Not_Called_For_A_Failure()
    {
        Ex093_MvuxFeedComposition.ResetProjections();

        await Outcome(Ex093_MvuxFeedComposition.Format(Failing()));

        // Which is the point of composing rather than hand-rolling: no downstream code has
        // to check for a value that is not there.
        Assert.Equal(0, Ex093_MvuxFeedComposition.Projections);
    }

    [Fact]
    public async Task The_Projection_Runs_Once_For_A_Value()
    {
        Ex093_MvuxFeedComposition.ResetProjections();

        await Outcome(Ex093_MvuxFeedComposition.Format(Number(1)));

        Assert.Equal(1, Ex093_MvuxFeedComposition.Projections);
    }

    [Fact]
    public async Task A_Filter_Passes_What_Qualifies()
    {
        var outcome = await Outcome(
            Ex093_MvuxFeedComposition.Format(Ex093_MvuxFeedComposition.AtLeast(Number(50), minimum: 10)));

        Assert.Equal("#50", outcome.Value);
    }

    [Fact]
    public async Task A_Filter_Excluding_A_Value_Produces_A_Deliberate_Absence()
    {
        var outcome = await Outcome(
            Ex093_MvuxFeedComposition.Format(Ex093_MvuxFeedComposition.AtLeast(Number(1), minimum: 10)));

        // None, not Undefined and not an error: a view can say "no matches" here, which a
        // single null could never express.
        Assert.True(outcome.IsEmpty);
        Assert.False(outcome.HasValue);
        Assert.Null(outcome.Error);
    }

    [Fact]
    public async Task A_Deliberate_Absence_Is_Not_A_Failure()
    {
        var excluded = await Outcome(
            Ex093_MvuxFeedComposition.Format(Ex093_MvuxFeedComposition.AtLeast(Number(1), minimum: 10)));
        var failed = await Outcome(Ex093_MvuxFeedComposition.Format(Failing()));

        Assert.NotEqual(excluded.IsEmpty, failed.IsEmpty);
    }

    [Fact]
    public async Task Combining_Pairs_The_Two_Feeds()
    {
        var outcome = await Outcome(
            Ex093_MvuxFeedComposition.Describe(Text("answer"), Number(42)));

        Assert.Equal("answer=42", outcome.Value);
    }

    [Fact]
    public async Task Either_Failure_Fails_The_Pair()
    {
        var outcome = await Outcome(
            Ex093_MvuxFeedComposition.Describe(Text("answer"), Failing()));

        Assert.Equal("boom", outcome.Error);
        Assert.False(outcome.HasValue);
    }

    [Fact]
    public async Task A_Combination_Of_Values_Carries_No_Error()
    {
        var outcome = await Outcome(
            Ex093_MvuxFeedComposition.Describe(Text("a"), Number(1)));

        Assert.Null(outcome.Error);
        Assert.False(outcome.IsEmpty);
    }
}
