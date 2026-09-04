// Exercise 067 - Debounced Input (intermediate).
// Goal:   Turn a burst of keystrokes into one search.
// Drills: coalescing rapid changes behind a delay, cancelling the pending one on the next
//         change, and injecting the delay so the behaviour can be tested without waiting.
// Passes: dotnet test --filter FullyQualifiedName~Ex067_
//
// The delay is a constructor parameter, not a Task.Delay call in the middle of the method.
// A hard-coded delay makes the test suite slow *and* flaky - and the seam costs one
// parameter.

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Calls an action once a burst of changes has settled.
/// </summary>
public sealed class Ex067_DebouncedInput
{
    private readonly Func<CancellationToken, Task> _delay;
    private readonly Func<string, CancellationToken, Task> _action;
    private CancellationTokenSource? _pending;

    /// <summary>
    /// <paramref name="delay"/> is how the debounce waits - in an app,
    /// <c>ct =&gt; Task.Delay(300, ct)</c>; in a test, something a test controls.
    /// </summary>
    public Ex067_DebouncedInput(Func<CancellationToken, Task> delay, Func<string, CancellationToken, Task> action)
    {
        _delay = delay;
        _action = action;
    }

    /// <summary>How many times the action has actually run.</summary>
    public int Runs { get; private set; }

    /// <summary>The value the action last ran with.</summary>
    public string? LastValue { get; private set; }

    /// <summary>
    /// Records a change. The action runs with <paramref name="value"/> once the delay has
    /// elapsed without another change arriving; a change that arrives first cancels this
    /// one, and a cancelled debounce never runs the action.
    /// </summary>
    public async Task ChangeAsync(string value) =>
        // TODO: cancel and replace the pending source (Cancel, not CancelAsync - see
        // uno/README.md), await the delay with the new token,
        // and run the action only if the token survived. OperationCanceledException from
        // the delay is the normal path here, not an error.
        throw new NotImplementedException("TODO: Ex067 - debounce the change");
}
