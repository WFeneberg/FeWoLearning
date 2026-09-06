// Exercise 046 - Coroutine Execution Context (intermediate).
// Goal:   CoroutineExecutionContext is how a coroutine step learns who invoked it (Target) and
//         against which view (View) - three settable properties (Source, View, Target), all
//         object-typed, all null on a directly-constructed context. Coroutine.ExecuteAsync hands
//         the SAME context instance to every step in a sequence, not a fresh copy per step.
// Drills: writing IResult.Execute so it reads context.Target and context.View (not context.Source
//         - a copy-paste mix-up between the three is the easy mistake here) and stashes what it
//         saw, so a caller can assert on it after the coroutine finishes.
// Passes: dotnet test --filter FullyQualifiedName~Ex046_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A hand-written coroutine step that captures the context it was executed with, so a
/// test can inspect afterwards what Target/View it actually saw.</summary>
public class Ex046_ContextAwareStep : IResult
{
    public object? SeenTarget { get; private set; }
    public object? SeenView { get; private set; }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    private void RaiseCompleted() => Completed?.Invoke(this, new ResultCompletionEventArgs());

    public void Execute(CoroutineExecutionContext context)
    {
        SeenTarget = context.Target;
        SeenView = context.View;
        RaiseCompleted();
    }
}

public class Ex046_CoroutineExecutionContext
{
    /// <summary>Runs steps, in order, through Coroutine.ExecuteAsync against context.</summary>
    public static Task RunAsync(IEnumerable<IResult> steps, CoroutineExecutionContext context) =>
        Coroutine.ExecuteAsync(steps.GetEnumerator(), context);
}
