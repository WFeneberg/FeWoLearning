using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex046_CoroutineExecutionContextTests : CaliburnCoreContext
{
    // BoundedAsync (used below) lives on CaliburnCoreContext - a step whose Execute never
    // raises Completed stalls Coroutine.ExecuteAsync forever, not a failure (same trap as
    // ex041-045).

    /// <summary>A step that mutates context.Target instead of reading it - used only to prove
    /// the SAME context instance flows to every step in a sequence, not a fresh copy per step.</summary>
    class MutatingStep(object newTarget) : IResult
    {
        public event EventHandler<ResultCompletionEventArgs>? Completed;

        public void Execute(CoroutineExecutionContext context)
        {
            context.Target = newTarget;
            Completed?.Invoke(this, new ResultCompletionEventArgs());
        }
    }

    [Fact]
    public async Task Execute_Captures_The_Contexts_Target_And_View()
    {
        var target = new object();
        var view = new object();
        var context = new CoroutineExecutionContext { Target = target, View = view };
        var step = new Ex046_ContextAwareStep();

        await BoundedAsync(Ex046_CoroutineExecutionContext.RunAsync([step], context), "single step");

        Assert.Same(target, step.SeenTarget);
        Assert.Same(view, step.SeenView);
    }

    [Fact]
    public async Task A_Directly_Constructed_Contexts_Null_Target_And_View_Are_Reported_As_Null_Not_Fabricated()
    {
        var context = new CoroutineExecutionContext();
        var step = new Ex046_ContextAwareStep();

        await BoundedAsync(Ex046_CoroutineExecutionContext.RunAsync([step], context), "single step, unset context");

        Assert.Null(step.SeenTarget);
        Assert.Null(step.SeenView);
    }

    [Fact]
    public async Task Setting_Source_Does_Not_Get_Confused_With_Target_Or_View()
    {
        // A stub that reads context.Source into SeenTarget (or vice versa, a copy-paste
        // mistake between the three properties) fails this even though the previous two
        // tests might still pass.
        var context = new CoroutineExecutionContext
        {
            Source = "the-source",
            Target = "the-target",
            View = "the-view",
        };
        var step = new Ex046_ContextAwareStep();

        await BoundedAsync(Ex046_CoroutineExecutionContext.RunAsync([step], context), "single step, all three set");

        Assert.Equal("the-target", step.SeenTarget);
        Assert.Equal("the-view", step.SeenView);
    }

    [Fact]
    public async Task Every_Step_In_A_Sequence_Receives_The_Same_Context_Instance()
    {
        // The middle step mutates context.Target; if step3 sees the MUTATED value (not the
        // original), that proves Coroutine.ExecuteAsync hands every step the same context
        // object, not a clone taken once at the start.
        var context = new CoroutineExecutionContext { Target = "original" };
        var step1 = new Ex046_ContextAwareStep();
        var mutator = new MutatingStep("mutated-by-step2");
        var step3 = new Ex046_ContextAwareStep();

        await BoundedAsync(
            Ex046_CoroutineExecutionContext.RunAsync([step1, mutator, step3], context),
            "three-step sequence with a mutation in the middle");

        Assert.Equal("original", step1.SeenTarget);
        Assert.Equal("mutated-by-step2", step3.SeenTarget);
    }

    [Fact]
    public async Task Two_Independent_Steps_In_The_Same_Sequence_Both_Capture_The_Contexts_Values()
    {
        var context = new CoroutineExecutionContext { Target = "shared-target", View = "shared-view" };
        var stepA = new Ex046_ContextAwareStep();
        var stepB = new Ex046_ContextAwareStep();

        await BoundedAsync(Ex046_CoroutineExecutionContext.RunAsync([stepA, stepB], context), "two independent steps");

        Assert.Equal("shared-target", stepA.SeenTarget);
        Assert.Equal("shared-target", stepB.SeenTarget);
        Assert.Equal("shared-view", stepA.SeenView);
        Assert.Equal("shared-view", stepB.SeenView);
    }
}
