// Exercise 041 - Coroutine Basics (intermediate).
// Goal:   IResult is Caliburn's coroutine step: a single Execute call that does the work, and a
//         Completed event that is the ONLY thing telling the coroutine engine "move on". Forgetting
//         to raise it does not fail anything by itself - it leaves the coroutine waiting forever,
//         because the Task Coroutine hands back only completes once Completed fires.
// Drills: hand-writing IResult.Execute for the first time - doing the work AND raising
//         Completed(this, new ResultCompletionEventArgs()) yourself; nothing does it for you.
// Passes: dotnet test --filter FullyQualifiedName~Ex041_
//
// Measured on this machine (Caliburn.Micro 5.0.258): IResult itself has exactly two members -
// Execute and the Completed event; there is no return value and no template method on the
// interface to override (the OnCompleted() below is this file's own helper, not part of
// IResult). Caliburn.Micro.TaskExtensions.ExecuteAsync(this IResult, CoroutineExecutionContext =
// null) is the extension that runs a single IResult and hands back the Task that completes only
// when Completed does - the tests use it directly, as `step.ExecuteAsync()`.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A hand-written coroutine step: appends its own Name to a shared log, then must
/// raise Completed so whatever is running it knows to move on.</summary>
public class Ex041_LoggingResult : IResult
{
    private List<string> Log { get; }

    public string Name { get; }

    public Ex041_LoggingResult(List<string> log, string name)
    {
        Log = log;
        Name = name;
    }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    /// <summary>Raises Completed - already wired for you; call this once your own work is done.</summary>
    private void OnCompleted() => Completed?.Invoke(this, new ResultCompletionEventArgs());

    /// <summary>The TODO: append Name to Log, then call OnCompleted() - nothing else does it.</summary>
    public void Execute(CoroutineExecutionContext context) =>
        throw new NotImplementedException("TODO: Ex041 - append Name to Log, then call OnCompleted()");
}
