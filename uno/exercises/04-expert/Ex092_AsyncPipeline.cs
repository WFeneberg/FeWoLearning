// Exercise 092 - Async Pipeline (expert).
// Goal:   Compose async steps into one operation that cancels and fails as a unit.
// Drills: chaining Func<T, CancellationToken, Task<U>>, one token through every step,
//         a failure that names the step it came from, and no step running after a cancel.
// Passes: dotnet test --filter FullyQualifiedName~Ex092_
//
// A pipeline is worth building rather than hand-chaining awaits for exactly two reasons:
// every step gets the same token without anybody remembering to pass it, and a failure can
// say *where* it happened. Hand-written chains lose the second one first - the stack trace
// says "await" and the log says "sequence contains no elements".

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>Thrown when a step fails, naming the step.</summary>
public sealed class Ex092_StepFailedException : Exception
{
    public Ex092_StepFailedException(string stepName, Exception inner)
        : base($"Step '{stepName}' failed: {inner.Message}", inner) =>
        StepName = stepName;

    /// <summary>The step that failed.</summary>
    public string StepName { get; }
}

/// <summary>
/// A sequence of named async steps from <typeparamref name="TInput"/> to whatever the last
/// step returns.
/// </summary>
public sealed class Ex092_AsyncPipeline<TInput, TOutput>
{
    private readonly List<string> _stepNames = [];
    private readonly Func<TInput, CancellationToken, Task<TOutput>> _run;

    private Ex092_AsyncPipeline(Func<TInput, CancellationToken, Task<TOutput>> run, IEnumerable<string> stepNames)
    {
        _run = run;
        _stepNames.AddRange(stepNames);
    }

    /// <summary>The names of the steps, in order.</summary>
    public IReadOnlyList<string> StepNames => _stepNames;

    /// <summary>A pipeline of one step.</summary>
    public static Ex092_AsyncPipeline<TInput, TOutput> Start(
        string name,
        Func<TInput, CancellationToken, Task<TOutput>> step) =>
        // TODO: wrap the step so a failure becomes an Ex092_StepFailedException naming it -
        // and so a cancellation stays a cancellation, which is not a failure.
        throw new NotImplementedException("TODO: Ex092 - start the pipeline");

    /// <summary>
    /// This pipeline followed by <paramref name="step"/>. The original is unchanged - a
    /// pipeline is a value, so two callers can extend the same base differently.
    /// </summary>
    public Ex092_AsyncPipeline<TInput, TNext> Then<TNext>(
        string name,
        Func<TOutput, CancellationToken, Task<TNext>> step) =>
        // TODO: compose. Check the token *between* the steps as well: a cancel that arrives
        // while step one is running must stop step two from starting at all.
        throw new NotImplementedException("TODO: Ex092 - append a step");

    /// <summary>Runs the pipeline.</summary>
    public Task<TOutput> RunAsync(TInput input, CancellationToken ct) => _run(input, ct);
}
