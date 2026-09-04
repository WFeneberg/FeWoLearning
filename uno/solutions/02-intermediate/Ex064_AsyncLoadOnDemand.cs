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
    public async Task LoadAsync(string query)
    {
        var previous = _pending;
        var current = new CancellationTokenSource();
        _pending = current;

        // Cancel the old request before starting the new one, and keep the new source in a
        // local: _pending will already point at a third request by the time this one
        // finishes, and comparing against the field is how the stale answer gets in.
        if (previous is not null)
        {
            await previous.CancelAsync();
            previous.Dispose();
        }

        State = Ex064_LoadState.Loading;

        try
        {
            var data = await _load(query, current.Token);

            // The second half of the fix: an answer whose request was cancelled belongs to
            // nobody and must not reach the screen.
            if (current.IsCancellationRequested)
            {
                return;
            }

            Data = data;
            Error = null;
            State = Ex064_LoadState.Loaded;
        }
        catch (OperationCanceledException)
        {
            // Not a failure - somebody asked for something else. The newer request owns
            // the state now, so this one leaves it alone.
        }
        catch (Exception error)
        {
            if (current.IsCancellationRequested)
            {
                return;
            }

            Error = error.Message;
            State = Ex064_LoadState.Failed;
        }
    }

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
