// Exercise 046 - From async void to a real async command (intermediate).
// Goal:   ICommand.Execute is stuck returning void forever - that boundary cannot change.
//         What CAN change is everything behind it: instead of an async void body with no way
//         for a caller (or a test) to know when it finished, expose the real work as a method
//         that returns a Task, gate it with an IsExecuting flag so a second press while one run
//         is still in flight does not start a second one, and make Execute a thin, unavoidable
//         fire-and-forget shim over that real entry point rather than the async void body
//         itself.
// Drills: an IsExecuting flag that both CanExecute and the entry point itself consult, raising
//         CanExecuteChanged on BOTH edges (starting AND finishing - announcing only the disable
//         leaves a bound button greyed out forever), capturing an exception the operation
//         throws instead of letting it reach ICommand.Execute's void boundary unhandled, and
//         refusing (not queuing, not blocking) a second start while one is already running.
// Passes: dotnet test --filter FullyQualifiedName~Ex046_

using System.ComponentModel;
using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ships only the command plumbing - no concrete operation here (one belongs to whoever
/// actually has work to run; shipping one "ready to use" would let this row's whole subject go
/// untested through it instead of through the base class itself). A subclass supplies the work
/// via <see cref="RunAsync"/>.
/// </summary>
public abstract class Ex046_AsyncCommandBase : ICommand, INotifyPropertyChanged
{
    public event EventHandler? CanExecuteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>True from the moment a run is accepted until it finishes, one way or another.</summary>
    public bool IsExecuting { get; private set; }

    /// <summary>The exception the most recent run failed with, or null if it succeeded (or
    /// hasn't run yet).</summary>
    public Exception? LastError { get; private set; }

    /// <summary>How many runs have actually been started (accepted, not refused).</summary>
    public int RunCount { get; private set; }

    /// <summary>The operation a subclass wants this command to run. May throw - the base class
    /// catches it.</summary>
    protected abstract Task RunAsync(object? parameter);

    public bool CanExecute(object? parameter) =>
        throw new NotImplementedException("TODO: Ex046 - false while IsExecuting is true, true otherwise");

    /// <summary>
    /// The real entry point - returns the Task representing this run, so a caller (or a test)
    /// can await completion directly instead of needing an async void handler anywhere. Refuses
    /// (returns an already-completed task, does not start a second RunAsync) while a previous
    /// run is still in flight. Otherwise: sets IsExecuting, raises PropertyChanged(nameof(IsExecuting))
    /// and CanExecuteChanged, increments RunCount, runs RunAsync, captures any exception it
    /// throws into LastError (clearing LastError first, so a run that succeeds after an earlier
    /// failure does not leave the stale exception behind), then - regardless of success or
    /// failure - clears IsExecuting and raises both notifications again.
    /// </summary>
    public Task ExecuteAsync(object? parameter) =>
        throw new NotImplementedException("TODO: Ex046 - refuse a second concurrent run; otherwise set IsExecuting, call RaisePropertyChanged(nameof(IsExecuting)) and RaiseCanExecuteChanged, run RunAsync capturing its exception into LastError (clearing LastError on a run that does not throw), then always clear IsExecuting and raise both notifications again on the way out, including the failure path");

    /// <summary>
    /// ICommand's unavoidable void boundary - a thin, fire-and-forget shim over
    /// <see cref="ExecuteAsync"/>, which is where all the real logic (and its own exception
    /// handling) lives. Ready to use.
    /// </summary>
    void ICommand.Execute(object? parameter) => _ = ExecuteAsync(parameter);

    /// <summary>Raises PropertyChanged. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Raises CanExecuteChanged. Ready to use.</summary>
    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
