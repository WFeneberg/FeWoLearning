using FeWoLearning.Architecture.Exercises.CrossCutting.Ex053;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex053_ResiliencePipelineTests
{
    private static readonly TimeSpan Cost = TimeSpan.FromMilliseconds(400);

    private static (ManualClock Clock, TimedWork Work) Build(int succeedsOnAttempt, TimeSpan? cost = null)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (clock, new TimedWork(clock.Advance, cost ?? Cost, succeedsOnAttempt));
    }

    [Fact]
    public void Retry_Outside_Timeout_Keeps_Trying_Until_The_Attempt_Budget_Runs_Out()
    {
        var (_, work) = Build(succeedsOnAttempt: 3);

        Assert.True(Ex053_ResiliencePipeline.RetryOutsideTimeout(work, maxAttempts: 3, TimeSpan.FromSeconds(1)));
        Assert.Equal(3, work.Attempts);
    }

    [Fact]
    public void Mechanism_The_Same_Work_Under_An_Outer_Timeout_Gives_Up_Sooner()
    {
        // The exercise. Same work, same retry budget, different composition - and a
        // different answer. Three attempts at 400 ms each need 1.2 s; an overall budget
        // of one second stops after two, with no third attempt started.
        var (clock, work) = Build(succeedsOnAttempt: 3);

        Assert.False(Ex053_ResiliencePipeline.TimeoutOutsideRetry(clock, work, maxAttempts: 3, TimeSpan.FromSeconds(1)));
        Assert.Equal(2, work.Attempts);
    }

    [Fact]
    public void Mechanism_An_Attempt_That_Overruns_Its_Own_Budget_Is_Retried()
    {
        // Retry on the outside means a timeout is just another failure. A pipeline that
        // let an overrun escape - or that treated it as final - would give up on exactly
        // the transient condition retries exist for.
        var (_, work) = Build(succeedsOnAttempt: 1, cost: TimeSpan.FromSeconds(2));

        Assert.False(Ex053_ResiliencePipeline.RetryOutsideTimeout(work, maxAttempts: 3, TimeSpan.FromSeconds(1)));
        Assert.Equal(3, work.Attempts);
    }

    [Fact]
    public void Mechanism_An_Outer_Timeout_Does_Not_Start_An_Attempt_It_Cannot_Finish()
    {
        // Checked before starting, not after. Starting an attempt that cannot finish
        // spends the caller's remaining patience on work whose result is discarded -
        // which is the whole reason the outer timeout is there.
        var (clock, work) = Build(succeedsOnAttempt: 5, cost: TimeSpan.FromSeconds(2));

        Assert.False(Ex053_ResiliencePipeline.TimeoutOutsideRetry(clock, work, maxAttempts: 5, TimeSpan.FromSeconds(1)));
        Assert.Equal(0, work.Attempts);
    }

    [Fact]
    public void Work_That_Succeeds_First_Time_Costs_One_Attempt_Either_Way()
    {
        // Pairs with the divergence fact: the two orderings must agree when nothing goes
        // wrong, or the difference measured above would just be one of them being broken.
        var (clockA, workA) = Build(succeedsOnAttempt: 1);
        var (clockB, workB) = Build(succeedsOnAttempt: 1);

        Assert.True(Ex053_ResiliencePipeline.RetryOutsideTimeout(workA, 3, TimeSpan.FromSeconds(1)));
        Assert.True(Ex053_ResiliencePipeline.TimeoutOutsideRetry(clockB, workB, 3, TimeSpan.FromSeconds(1)));

        Assert.Equal(1, workA.Attempts);
        Assert.Equal(1, workB.Attempts);
        Assert.Equal(clockA.UtcNow, clockB.UtcNow);
    }

    [Fact]
    public void Retry_Outside_Timeout_Can_Take_Far_Longer_Than_The_Per_Attempt_Budget()
    {
        // The cost of the ordering, made explicit: "retry three times with a one-second
        // timeout" can take three seconds, and a caller waiting on the other end of an
        // HTTP request is entitled to know that before it is deployed.
        var (clock, work) = Build(succeedsOnAttempt: 3, cost: TimeSpan.FromMilliseconds(900));

        Ex053_ResiliencePipeline.RetryOutsideTimeout(work, maxAttempts: 3, TimeSpan.FromSeconds(1));

        Assert.Equal(TimeSpan.FromMilliseconds(2700), clock.UtcNow - new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }
}
