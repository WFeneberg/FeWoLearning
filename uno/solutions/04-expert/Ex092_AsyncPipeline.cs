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
        new(
            async (input, ct) =>
            {
                // Checked before the step, so an already-cancelled token costs nothing.
                ct.ThrowIfCancellationRequested();

                return await Invoke(name, step, input, ct);
            },
            [name]);

    /// <summary>
    /// Runs one step, turning a failure into a named one and leaving a cancellation alone.
    /// </summary>
    private static async Task<TResult> Invoke<TStepInput, TResult>(
        string name,
        Func<TStepInput, CancellationToken, Task<TResult>> step,
        TStepInput input,
        CancellationToken ct)
    {
        try
        {
            return await step(input, ct);
        }
        catch (OperationCanceledException)
        {
            // Not a failure. Wrapping it would make every caller unwrap it again to find
            // out whether anything actually went wrong.
            throw;
        }
        catch (Exception error)
        {
            throw new Ex092_StepFailedException(name, error);
        }
    }

    /// <summary>
    /// This pipeline followed by <paramref name="step"/>. The original is unchanged - a
    /// pipeline is a value, so two callers can extend the same base differently.
    /// </summary>
    public Ex092_AsyncPipeline<TInput, TNext> Then<TNext>(
        string name,
        Func<TOutput, CancellationToken, Task<TNext>> step)
    {
        var previous = _run;

        return new Ex092_AsyncPipeline<TInput, TNext>(
            async (input, ct) =>
            {
                var intermediate = await previous(input, ct);

                // Between the steps: this is what makes cancellation work for a step that
                // never looks at the token itself.
                ct.ThrowIfCancellationRequested();

                return await Ex092_AsyncPipeline<TInput, TNext>.Invoke(name, step, intermediate, ct);
            },
            [.._stepNames, name]);
    }

    /// <summary>Runs the pipeline.</summary>
    public Task<TOutput> RunAsync(TInput input, CancellationToken ct) => _run(input, ct);
}
