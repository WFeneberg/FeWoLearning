using FeWoLearning.Architecture.Exercises.ServicesData.Ex040;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex040_SagaProcessManagerTests
{
    private static SagaStep Step(string name, List<string> log, bool fails = false) =>
        new(name,
            Execute: () =>
            {
                log.Add($"do:{name}");
                return fails ? Task.FromException(new InvalidOperationException(name + " failed")) : Task.CompletedTask;
            },
            Compensate: () =>
            {
                log.Add($"undo:{name}");
                return Task.CompletedTask;
            });

    [Fact]
    public async Task A_Saga_Where_Everything_Works_Compensates_Nothing()
    {
        var log = new List<string>();

        var result = await Ex040_SagaProcessManager.RunAsync(
            [Step("reserve", log), Step("charge", log), Step("ship", log)]);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Compensated);
        Assert.Equal(["do:reserve", "do:charge", "do:ship"], log);
    }

    [Fact]
    public async Task Mechanism_A_Failure_Unwinds_The_Completed_Steps_In_Reverse()
    {
        // The order is the fact. Compensating forward undoes step 1 while step 2 still
        // depends on it - refunding the payment before cancelling the shipment it paid
        // for - and an assertion that merely checks "both were compensated" is happy
        // either way.
        var log = new List<string>();

        var result = await Ex040_SagaProcessManager.RunAsync(
            [Step("reserve", log), Step("charge", log), Step("ship", log, fails: true), Step("notify", log)]);

        Assert.False(result.Succeeded);
        Assert.Equal("ship", result.FailedStep);
        Assert.Equal(["charge", "reserve"], result.Compensated);
        Assert.Equal(["do:reserve", "do:charge", "do:ship", "undo:charge", "undo:reserve"], log);
    }

    [Fact]
    public async Task Mechanism_The_Step_That_Failed_Is_Not_Compensated()
    {
        // It never completed, so undoing it means undoing something that never happened.
        // For a refund that is paying out money nobody was ever charged.
        var log = new List<string>();

        var result = await Ex040_SagaProcessManager.RunAsync(
            [Step("reserve", log), Step("charge", log, fails: true)]);

        Assert.DoesNotContain("undo:charge", log);
        Assert.DoesNotContain("charge", result.Compensated);
    }

    [Fact]
    public async Task Steps_After_The_Failure_Never_Run()
    {
        var log = new List<string>();

        await Ex040_SagaProcessManager.RunAsync(
            [Step("reserve", log, fails: true), Step("charge", log)]);

        Assert.DoesNotContain("do:charge", log);
    }

    [Fact]
    public async Task Adversarial_A_Failing_Compensation_Does_Not_Strand_The_Rest()
    {
        // There is nobody left to appeal to when an undo fails. Letting it throw stops
        // the unwind and strands every step below it as well, turning one piece of
        // manual cleanup into several - and the caller is told nothing about either.
        var log = new List<string>();

        var stubborn = new SagaStep("charge",
            Execute: () => { log.Add("do:charge"); return Task.CompletedTask; },
            Compensate: () => throw new InvalidOperationException("the refund API is down"));

        var result = await Ex040_SagaProcessManager.RunAsync(
            [Step("reserve", log), stubborn, Step("ship", log, fails: true)]);

        Assert.Equal(["charge"], result.CompensationFailures);
        Assert.Equal(["reserve"], result.Compensated);
        Assert.Contains("undo:reserve", log);
    }

    [Fact]
    public async Task An_Empty_Saga_Succeeds()
    {
        var result = await Ex040_SagaProcessManager.RunAsync([]);

        Assert.True(result.Succeeded);
        Assert.Null(result.FailedStep);
    }
}
