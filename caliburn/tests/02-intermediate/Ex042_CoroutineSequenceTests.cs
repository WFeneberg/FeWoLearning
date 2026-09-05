using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex042_CoroutineSequenceTests : CaliburnCoreContext
{
    // BoundedAsync (used below) lives on CaliburnCoreContext - see its comment for why a
    // coroutine await needs bounding at all.

    [Fact]
    public async Task Three_Steps_Run_Strictly_In_The_Order_Passed()
    {
        var log = new List<string>();
        var first = new Ex042_LoggingStep(log, "alpha");
        var second = new Ex042_LoggingStep(log, "beta");
        var third = new Ex042_LoggingStep(log, "gamma");

        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync");

        Assert.Equal(new[] { "alpha", "beta", "gamma" }, log);
    }

    [Fact]
    public async Task The_Order_Follows_The_Parameters_Not_A_Hardcoded_Sequence()
    {
        // Different names in the same first/second/third slots as the test above - a stub that
        // hardcodes which instance goes first (rather than yielding the parameters themselves)
        // would still pass the previous test but fail this one.
        var log = new List<string>();
        var first = new Ex042_LoggingStep(log, "gamma");
        var second = new Ex042_LoggingStep(log, "alpha");
        var third = new Ex042_LoggingStep(log, "beta");

        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync");

        Assert.Equal(new[] { "gamma", "alpha", "beta" }, log);
    }

    [Fact]
    public async Task All_Three_Steps_Run_Exactly_Once_Each()
    {
        var log = new List<string>();
        var first = new Ex042_LoggingStep(log, "one");
        var second = new Ex042_LoggingStep(log, "two");
        var third = new Ex042_LoggingStep(log, "three");

        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync");

        // A stub that drops a yield (e.g. only first and second) would leave this short of 3.
        Assert.Equal(3, log.Count);
    }

    [Fact]
    public async Task A_Slow_First_Step_Still_Finishes_Before_The_Second_Ever_Starts()
    {
        var log = new List<string>();
        var first = new Ex042_DelayedStep(log, "slow-start", "slow-done", TimeSpan.FromMilliseconds(150));
        var second = new Ex042_LoggingStep(log, "second");
        var third = new Ex042_LoggingStep(log, "third");

        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync");

        Assert.Equal(new[] { "slow-start", "slow-done", "second", "third" }, log);
    }

    [Fact]
    public async Task Running_The_Same_Sequence_Twice_Produces_Two_Independent_Full_Runs()
    {
        var log = new List<string>();
        var first = new Ex042_LoggingStep(log, "x");
        var second = new Ex042_LoggingStep(log, "y");
        var third = new Ex042_LoggingStep(log, "z");

        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync (first run)");
        await BoundedAsync(Ex042_CoroutineSequence.RunInOrderAsync(first, second, third), "RunInOrderAsync (second run)");

        Assert.Equal(new[] { "x", "y", "z", "x", "y", "z" }, log);
    }
}
