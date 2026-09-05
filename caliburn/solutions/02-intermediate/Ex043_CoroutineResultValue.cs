// Exercise 043 - Coroutine Result Value (intermediate).
// Goal:   IResult<T> adds exactly one thing over IResult: a Result property of type T. It has no
//         setter on the interface - Execute is responsible for computing the value and storing it
//         somewhere Result can read it BEFORE raising Completed, because nothing else hands the
//         value anywhere. Coroutine.ExecuteAsync still returns a plain Task, not a Task<T> - the
//         value only ever lives on the IResult<T> instance itself, read after the fact.
// Drills: writing IResult<T>.Execute so the *instance's own* Result reflects what happened, then
//         reading step.Result off that instance once the coroutine has completed.
// Passes: dotnet test --filter FullyQualifiedName~Ex043_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A hand-written value-returning coroutine step: computes Factory() and must make it
/// readable through Result before raising Completed.</summary>
public class Ex043_ValueResult<T> : IResult<T>
{
    private Func<T> Factory { get; }

    private T? StoredResult { get; set; }

    public Ex043_ValueResult(Func<T> factory) => Factory = factory;

    public T Result => StoredResult!;

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    private void OnCompleted() => Completed?.Invoke(this, new ResultCompletionEventArgs());

    public void Execute(CoroutineExecutionContext context)
    {
        StoredResult = Factory();
        OnCompleted();
    }
}
