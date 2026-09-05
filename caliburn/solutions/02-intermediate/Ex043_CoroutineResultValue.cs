// Exercise 043 - Coroutine Result Value (intermediate).
// Goal:   IResult<T> adds exactly one thing over IResult: a Result property of type T, read-only
//         on the interface - Execute is responsible for computing the value and storing it
//         somewhere Result can return, BEFORE raising Completed, because nothing else hands the
//         value anywhere. Inside a yield-return sequence, Coroutine.ExecuteAsync still returns a
//         plain Task, not a Task<T> - the value reaches you only through the instance's own
//         Result there. Run as a single step instead, though, and TaskExtensions (in
//         Caliburn.Micro) has a generic ExecuteAsync<TResult>(this IResult<TResult>, ...)
//         overload that DOES return Task<TResult> directly - awaiting it hands the value back
//         without ever touching Result yourself. Unlike ex041 (where raising Completed IS the
//         lesson, so it is left for you), OnCompleted here is already wired - Result is the
//         subject this time, and that means BOTH computing the value in Execute AND returning it
//         from Result are yours to write.
// Drills: writing IResult<T>.Execute so the *instance's own* Result reflects what happened, and
//         writing Result's own getter - both are part of IResult<T>'s surface, not just Execute.
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
