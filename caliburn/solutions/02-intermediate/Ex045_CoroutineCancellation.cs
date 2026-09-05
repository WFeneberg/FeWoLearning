// Exercise 045 - Coroutine Cancellation (intermediate).
// Goal:   A coroutine sequence stops early in exactly two ways, and Caliburn does NOT treat them
//         the same: a step whose ResultCompletionEventArgs.WasCancelled is true makes
//         Coroutine.ExecuteAsync's Task fault with a TaskCanceledException, while a step whose
//         ResultCompletionEventArgs.Error is set instead makes it fault with THAT SAME exception,
//         unwrapped. Either way, no later step in the sequence ever runs.
// Drills: writing IResult.Execute so it logs its own name, then reports failure through whichever
//         ResultCompletionEventArgs member the caller asked for - Cancel sets WasCancelled, Fail
//         sets Error, Succeed sets neither.
// Passes: dotnet test --filter FullyQualifiedName~Ex045_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public enum Ex045_Outcome
{
    Succeed,
    Cancel,
    Fail,
}

/// <summary>A hand-written coroutine step: logs Name, then must report Outcome through
/// ResultCompletionEventArgs - Cancel via WasCancelled, Fail via Error, Succeed via neither.</summary>
public class Ex045_OutcomeStep : IResult
{
    private readonly List<string> _log;
    private readonly Ex045_Outcome _outcome;
    private readonly Exception? _failure;

    public Ex045_OutcomeStep(List<string> log, string name, Ex045_Outcome outcome, Exception? failure = null)
    {
        _log = log;
        Name = name;
        _outcome = outcome;
        _failure = failure;
    }

    public string Name { get; }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    private void RaiseCompleted(ResultCompletionEventArgs args) => Completed?.Invoke(this, args);

    public void Execute(CoroutineExecutionContext context)
    {
        _log.Add(Name);

        var args = _outcome switch
        {
            Ex045_Outcome.Cancel => new ResultCompletionEventArgs { WasCancelled = true },
            Ex045_Outcome.Fail => new ResultCompletionEventArgs { Error = _failure },
            _ => new ResultCompletionEventArgs(),
        };

        RaiseCompleted(args);
    }
}

public class Ex045_CoroutineCancellation
{
    /// <summary>Runs steps, in order, through Coroutine.ExecuteAsync.</summary>
    public static Task RunAsync(IEnumerable<IResult> steps) =>
        Coroutine.ExecuteAsync(steps.GetEnumerator(), new CoroutineExecutionContext());
}
