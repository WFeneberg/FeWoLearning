// Exercise 093 - Async Stream View Model (expert).
// Goal:   Drive a view model from an IAsyncEnumerable and stop cleanly.
// Drills: await foreach with a cancellation token, a collection that only grows on the
//         consuming side, completion and failure as distinct end states, and a second
//         subscription superseding the first.
// Passes: dotnet test --filter FullyQualifiedName~Ex093_
//
// A stream is the honest shape for anything that arrives over time - a server feed, a scan,
// a long import. The trap is that `await foreach` without a token is uncancellable: the
// loop only ends when the producer says so, and a page that navigated away five minutes ago
// is still appending to a collection nobody is looking at.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>How a stream ended.</summary>
public enum Ex093_StreamState
{
    /// <summary>Nothing has been consumed yet.</summary>
    Idle,

    /// <summary>Items are arriving.</summary>
    Running,

    /// <summary>The producer finished.</summary>
    Completed,

    /// <summary>The producer threw.</summary>
    Failed,

    /// <summary>The consumer stopped listening.</summary>
    Stopped,
}

/// <summary>
/// Consumes a stream of strings into an observable collection.
/// </summary>
public sealed class Ex093_AsyncStreamViewModel : INotifyPropertyChanged
{
    private CancellationTokenSource? _pending;
    private Ex093_StreamState _state = Ex093_StreamState.Idle;
    private string? _error;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>The items received so far, in arrival order.</summary>
    public ObservableCollection<string> Items { get; } = [];

    public Ex093_StreamState State
    {
        get => _state;
        private set => Set(ref _state, value);
    }

    /// <summary>The producer's failure message, or null.</summary>
    public string? Error
    {
        get => _error;
        private set => Set(ref _error, value);
    }

    /// <summary>
    /// Consumes <paramref name="stream"/> into <see cref="Items"/>. A previous consumption
    /// is stopped first, and its remaining items never arrive. Ends in
    /// <see cref="Ex093_StreamState.Completed"/>, <see cref="Ex093_StreamState.Failed"/> or
    /// - when superseded or stopped - <see cref="Ex093_StreamState.Stopped"/>.
    /// </summary>
    public async Task ConsumeAsync(IAsyncEnumerable<string> stream) =>
        // TODO: swap the token source (Cancel, not CancelAsync - see uno/README.md), clear
        // the items, set the state, and `await foreach` with the token. Sort the three
        // endings out: a producer failure is Failed with its message, a cancellation is
        // Stopped, and reaching the end is Completed.
        //
        // A superseded run must not write anything after the newer one started - the same
        // discipline as ex064.
        throw new NotImplementedException("TODO: Ex093 - consume the stream");

    /// <summary>Stops consuming. Harmless when nothing is running.</summary>
    public void Stop() =>
        throw new NotImplementedException("TODO: Ex093 - stop consuming");

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
