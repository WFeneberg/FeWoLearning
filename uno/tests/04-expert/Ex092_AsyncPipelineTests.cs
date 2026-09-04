using FeWoLearning.Uno.Exercises.Expert;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex092_AsyncPipelineTests : UnoTestContext
{
    private static Ex092_AsyncPipeline<int, string> TwoSteps(List<string>? trace = null) =>
        Ex092_AsyncPipeline<int, int>
            .Start("double", (input, _) =>
            {
                trace?.Add("double");
                return Task.FromResult(input * 2);
            })
            .Then("format", (input, _) =>
            {
                trace?.Add("format");
                return Task.FromResult($"#{input}");
            });

    [Fact]
    public async Task A_Single_Step_Runs()
    {
        var pipeline = Ex092_AsyncPipeline<int, int>.Start("double", (input, _) => Task.FromResult(input * 2));

        Assert.Equal(6, await pipeline.RunAsync(3, CancellationToken.None));
    }

    [Fact]
    public async Task Steps_Run_In_Order()
    {
        var trace = new List<string>();

        var result = await TwoSteps(trace).RunAsync(3, CancellationToken.None);

        Assert.Equal("#6", result);
        Assert.Equal(["double", "format"], trace);
    }

    [Fact]
    public void The_Step_Names_Are_Recorded_In_Order()
    {
        Assert.Equal(["double", "format"], TwoSteps().StepNames);
    }

    [Fact]
    public async Task A_Failing_Step_Names_Itself()
    {
        var pipeline = Ex092_AsyncPipeline<int, int>
            .Start("double", (input, _) => Task.FromResult(input * 2))
            .Then<string>("format", (_, _) => Task.FromException<string>(new InvalidOperationException("boom")));

        var error = await Assert.ThrowsAsync<Ex092_StepFailedException>(
            () => pipeline.RunAsync(3, CancellationToken.None));

        // The reason to build this rather than chain awaits by hand: a hand-written chain
        // fails with a stack trace full of `await` and a message with no context.
        Assert.Equal("format", error.StepName);
        Assert.Equal("boom", error.InnerException!.Message);
    }

    [Fact]
    public async Task A_Failure_Stops_The_Pipeline()
    {
        var trace = new List<string>();
        var pipeline = Ex092_AsyncPipeline<int, int>
            .Start("fail", (_, _) => Task.FromException<int>(new InvalidOperationException("boom")))
            .Then("never", (input, _) =>
            {
                trace.Add("never");
                return Task.FromResult(input);
            });

        await Assert.ThrowsAsync<Ex092_StepFailedException>(() => pipeline.RunAsync(3, CancellationToken.None));

        Assert.Empty(trace);
    }

    [Fact]
    public async Task A_Cancelled_Token_Stops_Before_The_First_Step()
    {
        var trace = new List<string>();
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => TwoSteps(trace).RunAsync(3, source.Token));

        Assert.Empty(trace);
    }

    [Fact]
    public async Task A_Cancel_Between_Steps_Stops_The_Next_One()
    {
        var trace = new List<string>();
        using var source = new CancellationTokenSource();

        var pipeline = Ex092_AsyncPipeline<int, int>
            .Start("first", (input, _) =>
            {
                trace.Add("first");
                source.Cancel();
                return Task.FromResult(input);
            })
            .Then("second", (input, _) =>
            {
                trace.Add("second");
                return Task.FromResult(input);
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pipeline.RunAsync(3, source.Token));

        // The step itself never observed the token - the pipeline checked between steps,
        // which is what makes cancellation work for steps that do not check at all.
        Assert.Equal(["first"], trace);
    }

    [Fact]
    public async Task A_Cancellation_Is_Not_A_Step_Failure()
    {
        using var source = new CancellationTokenSource();
        var pipeline = Ex092_AsyncPipeline<int, int>.Start("cancel", (_, ct) =>
        {
            source.Cancel();
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(0);
        });

        // Wrapping a cancellation as a failure makes every caller unwrap it again to find
        // out whether anything actually went wrong.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pipeline.RunAsync(3, source.Token));
    }

    [Fact]
    public async Task The_Token_Reaches_Every_Step()
    {
        var seen = new List<bool>();
        using var source = new CancellationTokenSource();
        var pipeline = Ex092_AsyncPipeline<int, int>
            .Start("first", (input, ct) =>
            {
                seen.Add(ct.CanBeCanceled);
                return Task.FromResult(input);
            })
            .Then("second", (input, ct) =>
            {
                seen.Add(ct.CanBeCanceled);
                return Task.FromResult(input);
            });

        await pipeline.RunAsync(3, source.Token);

        Assert.Equal([true, true], seen);
    }

    [Fact]
    public async Task Extending_A_Pipeline_Leaves_The_Original_Alone()
    {
        var basePipeline = Ex092_AsyncPipeline<int, int>.Start("double", (input, _) => Task.FromResult(input * 2));

        var extended = basePipeline.Then("format", (input, _) => Task.FromResult($"#{input}"));

        // A pipeline is a value: two callers can extend the same base differently.
        Assert.Equal(6, await basePipeline.RunAsync(3, CancellationToken.None));
        Assert.Equal("#6", await extended.RunAsync(3, CancellationToken.None));
        Assert.Single(basePipeline.StepNames);
    }
}
