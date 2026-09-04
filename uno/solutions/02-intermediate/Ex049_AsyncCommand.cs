// Exercise 049 - Async Command (intermediate).
// Goal:   Run an awaitable operation from a command without letting it run twice.
// Drills: ICommand.Execute being void over an async body, a busy flag that gates
//         CanExecute, capturing the exception a fire-and-forget task would have swallowed,
//         and raising CanExecuteChanged on both edges.
// Passes: dotnet test --filter FullyQualifiedName~Ex049_
//
// Execute returns void, so nothing awaits it and nothing catches what it throws. An
// unhandled exception in an async void handler does not fail a command, it takes the
// process down - which is why this class exists in every MVVM toolkit.

using System.Windows.Input;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Runs an async operation at most once at a time, and remembers how it ended.
/// </summary>
public sealed class Ex049_AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;

    public Ex049_AsyncCommand(Func<Task> execute) => _execute = execute;

    public event EventHandler? CanExecuteChanged;

    /// <summary>True while the operation is in flight.</summary>
    public bool IsRunning { get; private set; }

    /// <summary>The exception the last run failed with, or null.</summary>
    public Exception? LastError { get; private set; }

    /// <summary>How many times the operation has actually been started.</summary>
    public int Started { get; private set; }

    /// <summary>False while the operation is running, so a bound button disables itself.</summary>
    public bool CanExecute(object? parameter) => !IsRunning;

    /// <summary>
    /// Starts the operation if it is not already running. Sets <see cref="IsRunning"/>,
    /// announces the change, awaits the work, captures any exception in
    /// <see cref="LastError"/>, then clears the flag and announces again.
    /// </summary>
    // async void, deliberately: ICommand.Execute returns void and there is no other shape
    // available. Everything below follows from that - nobody awaits this method, so nobody
    // can observe what it throws.
    public async void Execute(object? parameter)
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        LastError = null;
        Started++;
        RaiseCanExecuteChanged();

        try
        {
            await _execute();
        }
        catch (Exception error)
        {
            // Not a swallow: the exception is kept where a view model can bind to it. What
            // must not happen is it escaping an async void body, where it reaches the
            // runtime and takes the process down.
            LastError = error;
        }
        finally
        {
            // In the finally, so one failure does not disable the command for good.
            IsRunning = false;
            RaiseCanExecuteChanged();
        }
    }

    private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
