using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex043_CoroutineResultValueTests : CaliburnCoreContext
{
    // A step that never raises Completed makes the awaited Task wait forever, not fail - see
    // caliburn/README.md's Traps table. Bounding every coroutine await here means a forgotten
    // Completed shows up as a clear, fast failure instead of stalling the whole suite.
    private static async Task BoundedAsync(Task task, string because)
    {
        var winner = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.True(winner == task,
            $"Timed out waiting for {because} - a forgotten Completed stalls the coroutine forever instead of failing.");
        await task;
    }

    [Fact]
    public void Execute_Stores_The_Factorys_Value_Into_Result()
    {
        var step = new Ex043_ValueResult<int>(() => 42);

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal(42, step.Result);
    }

    [Fact]
    public void Execute_Calls_The_Factory_Rather_Than_Leaving_Result_At_A_Default()
    {
        // A stub that never calls _factory and leaves Result at default(int) would pass a test
        // asserting 0 - using a non-zero value here is what makes that failure mode visible.
        var step = new Ex043_ValueResult<int>(() => 7);

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal(7, step.Result);
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
    public void Execute_Raises_Completed_So_The_Coroutine_Knows_To_Move_On()
    {
        var step = new Ex043_ValueResult<int>(() => 1);
        var raised = false;
        step.Completed += (_, _) => raised = true;

        step.Execute(new CoroutineExecutionContext());

        Assert.True(raised);
    }
}
