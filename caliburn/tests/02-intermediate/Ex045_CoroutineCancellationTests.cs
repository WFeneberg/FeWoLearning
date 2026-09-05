using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex045_CoroutineCancellationTests : CaliburnCoreContext
{
    // BoundedExceptionAsync (used below) lives on CaliburnCoreContext - see its comment for why
    // a coroutine await needs bounding at all.

    [Fact]
    public async Task All_Three_Steps_Succeeding_Runs_Every_Step_In_Order_With_No_Exception()
    {
        var log = new List<string>();
        var steps = new IResult[]
        {
            new Ex045_OutcomeStep(log, "1", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(log, "2", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(log, "3", Ex045_Outcome.Succeed),
        };

        var ex = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(steps), "RunAsync (all succeed)");

        // A stub that always reports Cancel or Fail regardless of _outcome would fail this.
        Assert.Null(ex);
        Assert.Equal(new[] { "1", "2", "3" }, log);
    }

    [Fact]
    public async Task A_Cancelled_Second_Step_Stops_The_Third_And_Throws_TaskCanceledException()
    {
        var log = new List<string>();
        var steps = new IResult[]
        {
            new Ex045_OutcomeStep(log, "1", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(log, "2", Ex045_Outcome.Cancel),
            new Ex045_OutcomeStep(log, "3", Ex045_Outcome.Succeed),
        };

        var ex = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(steps), "RunAsync (cancelled)");

        Assert.IsType<TaskCanceledException>(ex);
        Assert.Equal(new[] { "1", "2" }, log);
    }

    [Fact]
    public async Task A_Failed_Second_Step_Stops_The_Third_And_Throws_The_Original_Exception_Type_And_Message()
    {
        var log = new List<string>();
        var steps = new IResult[]
        {
            new Ex045_OutcomeStep(log, "1", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(log, "2", Ex045_Outcome.Fail, new InvalidOperationException("boom")),
            new Ex045_OutcomeStep(log, "3", Ex045_Outcome.Succeed),
        };

        var ex = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(steps), "RunAsync (failed)");

        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("boom", ex!.Message);
        Assert.Equal(new[] { "1", "2" }, log);
    }

    [Fact]
    public async Task Cancellation_And_Failure_Produce_Genuinely_Different_Exception_Types()
    {
        var cancelLog = new List<string>();
        var failLog = new List<string>();
        var cancelSteps = new IResult[]
        {
            new Ex045_OutcomeStep(cancelLog, "1", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(cancelLog, "2", Ex045_Outcome.Cancel),
        };
        var failSteps = new IResult[]
        {
            new Ex045_OutcomeStep(failLog, "1", Ex045_Outcome.Succeed),
            new Ex045_OutcomeStep(failLog, "2", Ex045_Outcome.Fail, new ArgumentException("nope")),
        };

        var cancelEx = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(cancelSteps), "RunAsync (cancelled)");
        var failEx = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(failSteps), "RunAsync (failed)");

        Assert.NotEqual(cancelEx!.GetType(), failEx!.GetType());
        Assert.IsType<ArgumentException>(failEx);
    }

    [Fact]
    public async Task A_Step_That_Fails_First_Never_Lets_Any_Later_Step_Log_At_All()
    {
        var log = new List<string>();
        var steps = new IResult[]
        {
            new Ex045_OutcomeStep(log, "1", Ex045_Outcome.Cancel),
            new Ex045_OutcomeStep(log, "2", Ex045_Outcome.Succeed),
        };

        var ex = await BoundedExceptionAsync(Ex045_CoroutineCancellation.RunAsync(steps), "RunAsync (cancel first)");

        Assert.IsType<TaskCanceledException>(ex);
        Assert.Equal(new[] { "1" }, log);
    }
}
