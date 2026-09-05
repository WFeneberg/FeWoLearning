// Exercise 050 - Cancelling a running async command (intermediate).
// Goal:   A running command needs a way to be told to stop - a CancellationTokenSource created
//         fresh for each run, whose token is handed to the work, and whose Cancel() a caller
//         can invoke while that run is still in flight. A cancelled run is a NORMAL outcome
//         here, not a failure: it must be told apart from an actual exception, and it must
//         leave the command idle and ready to run again afterward, the same as any other
//         completed run.
// Drills: a fresh CancellationTokenSource per run, Cancel() reaching the CURRENTLY running
//         operation's token (not a stale one from a previous run, not none at all), telling
//         OperationCanceledException apart from a real failure, and the synchronous
//         CancellationTokenSource.Cancel() - NEVER `await CancelAsync()` - see wpf/README.md.
// Passes: dotnet test --filter FullyQualifiedName~Ex050_

using System.ComponentModel;
using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ships only the command plumbing - no concrete cancellable operation here (one belongs to
/// whoever actually has work to run; shipping one "ready to use" would let this row's whole
/// subject go untested through it instead of through the base class itself). A subclass
/// supplies the work via <see cref="RunAsync"/>, and must actually observe the token it is
/// given for cancellation to do anything.
/// </summary>
public abstract class Ex050_CancellableCommandBase : ICommand, INotifyPropertyChanged
{
    // No CancellationTokenSource field here - it does not exist to warn about (unread/
    // unassigned) until the learner adds it. Describe it in the TODO instead; see
    // wpf/README.md's "stub shape and compiler warnings" entry.

    public event EventHandler? CanExecuteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>True from the moment a run is accepted until it finishes, one way or another.</summary>
    public bool IsExecuting { get; private set; }

    /// <summary>True if the MOST RECENT run ended because it was cancelled - reset to false at
    /// the start of every new run.</summary>
    public bool WasCancelled { get; private set; }

    /// <summary>The exception the most recent run failed with - null on a run that succeeded
    /// OR was cancelled. Cancellation is not a failure.</summary>
    public Exception? LastError { get; private set; }

    /// <summary>The operation a subclass wants this command to run - must respect
    /// <paramref name="cancellationToken"/> for <see cref="Cancel"/> to have any effect.</summary>
    protected abstract Task RunAsync(CancellationToken cancellationToken);

    public bool CanExecute(object? parameter) =>
        throw new NotImplementedException("TODO: Ex050 - false while IsExecuting is true, true otherwise");

    /// <summary>
    /// Refuses (returns an already-completed task) while a previous run is still in flight.
    /// Otherwise: creates a fresh CancellationTokenSource for THIS run and remembers it (so
    /// Cancel() below can find it), sets IsExecuting, resets WasCancelled and LastError, calls
    /// RaisePropertyChanged(nameof(IsExecuting)) and RaiseCanExecuteChanged, runs RunAsync with
    /// that source's token, and on the way out: an OperationCanceledException from RunAsync
    /// sets WasCancelled (NOT LastError - that is the normal outcome of a cancelled run, not a
    /// failure), any OTHER exception sets LastError, and either way IsExecuting is cleared, the
    /// CancellationTokenSource is disposed and forgotten, and both notifications are raised
    /// again.
    /// </summary>
    public Task ExecuteAsync(object? parameter) =>
        throw new NotImplementedException("TODO: Ex050 - refuse a second concurrent run; otherwise create and remember a fresh CancellationTokenSource for this run, set IsExecuting/reset WasCancelled and LastError, raise both notifications, run RunAsync with this run's token, telling OperationCanceledException (-> WasCancelled) apart from any other exception (-> LastError), then always clear IsExecuting, dispose and forget the CancellationTokenSource, and raise both notifications again");

    /// <summary>
    /// Requests cancellation of whatever run is CURRENTLY in flight - a no-op if nothing is
    /// running. Must call the SYNCHRONOUS CancellationTokenSource.Cancel() - never
    /// `await ....CancelAsync()` - see wpf/README.md.
    /// </summary>
    public void Cancel() =>
        throw new NotImplementedException("TODO: Ex050 - if a run is currently in flight, call the synchronous Cancel() (not CancelAsync) on ITS CancellationTokenSource; otherwise do nothing");

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
