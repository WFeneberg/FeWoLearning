// Exercise 064 - Async Load On Demand (intermediate).
// Goal:   Load data on request, expose the three states a UI needs, and cancel a load that
//         has been superseded.
// Drills: a small state machine over an async operation, CancellationTokenSource swapping,
//         and not letting a slow first request overwrite a fast second one.
// Passes: dotnet test --filter FullyQualifiedName~Ex064_
//
// The bug this exercise is about outlives every framework: request A, request B, B answers
// first, then A answers and the screen shows A's stale data. Cancelling the previous token
// is half the fix; ignoring a result whose token was cancelled is the other half.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>What a loader is currently doing.</summary>
public enum Ex064_LoadState
{
    /// <summary>Nothing has been asked for yet.</summary>
    Idle,

    /// <summary>A load is in flight.</summary>
    Loading,

    /// <summary>The last load produced data.</summary>
    Loaded,

    /// <summary>The last load failed.</summary>
    Failed,
}

/// <summary>
/// Loads a string for a query, one load at a time, with the newest request winning.
/// </summary>
public sealed class Ex064_AsyncLoadOnDemand : INotifyPropertyChanged
{
    private readonly Func<string, CancellationToken, Task<string>> _load;
    private CancellationTokenSource? _pending;
    private Ex064_LoadState _state = Ex064_LoadState.Idle;
    private string? _data;
    private string? _error;

    public Ex064_AsyncLoadOnDemand(Func<string, CancellationToken, Task<string>> load) => _load = load;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Ex064_LoadState State
    {
        get => _state;
        private set => Set(ref _state, value);
    }

    /// <summary>The data from the last successful load.</summary>
    public string? Data
    {
        get => _data;
        private set => Set(ref _data, value);
    }

    /// <summary>The message from the last failed load.</summary>
    public string? Error
    {
        get => _error;
        private set => Set(ref _error, value);
    }

    /// <summary>
    /// Starts a load for <paramref name="query"/>, cancelling any load still in flight.
    /// Sets State to Loading, then to Loaded with Data, or to Failed with Error. A load
    /// that was cancelled changes nothing - its answer belongs to a request nobody wants.
    /// </summary>
    public async Task LoadAsync(string query) =>
        // TODO: swap the CancellationTokenSource (cancel and dispose the old one - use
        // Cancel, not CancelAsync, see uno/README.md), set the
        // state, await the work, and decide what to do with each of the three outcomes:
        // success, cancellation, failure. OperationCanceledException is not a failure.
        throw new NotImplementedException("TODO: Ex064 - load, cancelling what came before");

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
