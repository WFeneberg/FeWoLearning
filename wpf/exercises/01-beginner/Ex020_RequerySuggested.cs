// Exercise 020 - CommandManager.InvalidateRequerySuggested (beginner).
// Goal:   Watch a command's CanExecuteChanged the way WPF itself does - by forwarding
//         to the process-global CommandManager.RequerySuggested - and see the one rule
//         that governs it: a second InvalidateRequerySuggested() call while one is
//         still pending does not produce a second notification.
// Drills: CommandManager.InvalidateRequerySuggested and its coalescing behaviour
//         (it posts at DispatcherPriority.Background, and a second call before that
//         post has run is simply swallowed), plus weak handler storage: this class's
//         own subscription must be kept in a field, because
//         CommandManager.RequerySuggested - which Ex005_RelayCommand's
//         CanExecuteChanged forwards straight to - holds its handlers weakly, the same
//         as it did in ex005.
//
// A note on why every assertion here is a per-instance delta: CommandManager is
// process-global, and ex005's own tests leave a handler subscribed on it for the rest
// of the run - so no test anywhere may assert an exact *global* count of
// RequerySuggested/CanExecuteChanged notifications. Every count here is this class's
// own Count property, which starts at 0 for a fresh instance no matter what else is
// subscribed elsewhere.
// Passes: dotnet test --filter FullyQualifiedName~Ex020_

using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Counts how many times <paramref name="command"/>'s CanExecuteChanged actually
/// notifies this observer.
/// </summary>
public sealed class Ex020_RequeryObserver : IDisposable
{
    private readonly ICommand _command;

    // TODO: add a `private readonly EventHandler? _handler;` field here. Storing the
    // delegate in a field of your own - rather than subscribing an inline lambda kept
    // nowhere else - is what keeps it reachable between calls: CommandManager.
    // RequerySuggested holds its handlers weakly, so a delegate with no other strong
    // reference can be collected before it ever fires again.

    /// <summary>How many notifications this observer has received. Starts at 0.</summary>
    public int Count { get; private set; }

    public Ex020_RequeryObserver(ICommand command)
    {
        _command = command;

        // TODO: assign your _handler field to a delegate that increments Count (for
        // example (_, _) => Count++), then subscribe it to _command.CanExecuteChanged.
        throw new NotImplementedException("TODO: Ex020 - store a counting handler in a field and subscribe it to command.CanExecuteChanged");
    }

    public void Dispose()
    {
        // TODO: unsubscribe your stored handler from _command.CanExecuteChanged.
        throw new NotImplementedException("TODO: Ex020 - unsubscribe the stored handler from command.CanExecuteChanged");
    }
}
