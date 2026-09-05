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
//
// Measured on this machine (Caliburn.Micro 5.0.258), a three-step sequence logging 1, 2, 3: if
// step 2 sets WasCancelled, steps 1 and 2 run, step 3 does not, and ExecuteAsync throws
// TaskCanceledException; if step 2 instead sets Error to some exception, steps 1 and 2 run, step
// 3 does not, and ExecuteAsync throws THAT SAME exception - type and message intact. Two
// different failure modes, two distinguishable exception types.

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
    private List<string> Log { get; }

    public string Name { get; }

    private Ex045_Outcome Outcome { get; }

    private Exception? Failure { get; }

    public Ex045_OutcomeStep(List<string> log, string name, Ex045_Outcome outcome, Exception? failure = null)
    {
        Log = log;
        Name = name;
        Outcome = outcome;
        Failure = failure;
    }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    /// <summary>Raises Completed with the given args - already wired for you; build the right
    /// ResultCompletionEventArgs for Outcome and pass it here.</summary>
    private void RaiseCompleted(ResultCompletionEventArgs args) => Completed?.Invoke(this, args);

    /// <summary>The TODO: log Name, then call RaiseCompleted with args matching Outcome.</summary>
    public void Execute(CoroutineExecutionContext context) =>
        throw new NotImplementedException("TODO: Ex045 - log Name, then RaiseCompleted according to Outcome");
}

public class Ex045_CoroutineCancellation
{
    /// <summary>Runs steps, in order, through Coroutine.ExecuteAsync.</summary>
    public static Task RunAsync(IEnumerable<IResult> steps) =>
        Coroutine.ExecuteAsync(steps.GetEnumerator(), new CoroutineExecutionContext());
}
