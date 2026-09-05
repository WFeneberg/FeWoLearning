using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex043_CoroutineResultValueTests : CaliburnCoreContext
{
    [Fact]
    public void Execute_Stores_The_Factorys_Value_Into_Result()
    {
        var step = new Ex043_ValueResult<int>(() => 42);

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal(42, step.Result);
    }

    [Fact]
    public void Execute_Calls_The_Factory_Exactly_Once_Not_Once_Per_Read()
    {
        // Catches two distinct bugs at once: a stub that never calls Factory at all (callCount
        // stays 0), and a Result getter that re-evaluates Factory on every read instead of
        // returning what Execute already stored (callCount would climb past 1 below).
        var callCount = 0;
        var step = new Ex043_ValueResult<int>(() =>
        {
            callCount++;
            return 7;
        });

        step.Execute(new CoroutineExecutionContext());
        _ = step.Result;
        _ = step.Result;

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Works_For_Reference_Types_Too()
    {
        var step = new Ex043_ValueResult<string>(() => "hello");

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal("hello", step.Result);
    }

    [Fact]
    public void Two_Independent_Instances_Compute_Independent_Results()
    {
        var a = new Ex043_ValueResult<int>(() => 1);
        var b = new Ex043_ValueResult<int>(() => 2);

        a.Execute(new CoroutineExecutionContext());
        b.Execute(new CoroutineExecutionContext());

        Assert.Equal(1, a.Result);
        Assert.Equal(2, b.Result);
    }

    [Fact]
    public async Task Running_Through_Coroutine_ExecuteAsync_Still_Leaves_Result_Readable_Afterwards()
    {
        var step = new Ex043_ValueResult<int>(() => 99);

        await BoundedAsync(Coroutine.ExecuteAsync(Once(step), new CoroutineExecutionContext()),
            "Coroutine.ExecuteAsync(single step)");

        Assert.Equal(99, step.Result);

        static IEnumerator<IResult> Once(IResult single)
        {
            yield return single;
        }
    }

    [Fact]
    public async Task TaskExtensions_ExecuteAsync_Of_T_Hands_The_Value_Back_Directly()
    {
        // TaskExtensions.ExecuteAsync<TResult>(this IResult<TResult>, ...) is the single-step
        // convenience that returns Task<TResult> directly, unlike Coroutine.ExecuteAsync above.
        // The await is still bounded like every other coroutine await in this batch: an
        // implementation that stores the value but never raises Completed would otherwise hang
        // this test forever instead of failing it.
        var step = new Ex043_ValueResult<int>(() => 55);

        var value = await BoundedAsync(step.ExecuteAsync(), "ExecuteAsync<T> (single step)");

        Assert.Equal(55, value);
    }

    [Fact]
    public void Execute_Raises_Completed_So_The_Coroutine_Knows_To_Move_On()
    {
        var step = new Ex043_ValueResult<int>(() => 1);
        var raised = false;
        step.Completed += (_, _) => raised = true;

        step.Execute(new CoroutineExecutionContext());

        Assert.True(raised);
    }
}
