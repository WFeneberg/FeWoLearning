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
    public bool CanExecute(object? parameter) =>
        throw new NotImplementedException("TODO: Ex049 - refuse while running");

    /// <summary>
    /// Starts the operation if it is not already running. Sets <see cref="IsRunning"/>,
    /// announces the change, awaits the work, captures any exception in
    /// <see cref="LastError"/>, then clears the flag and announces again.
    /// </summary>
    public void Execute(object? parameter) =>
        // TODO: this is the one place an `async void` is the right shape - ICommand.Execute
        // has no other option. Which is exactly why the try/catch cannot be left out: there
        // is no caller to hand the exception to.
        throw new NotImplementedException("TODO: Ex049 - run the operation once, safely");

    private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
