// Exercise 020 - CommandManager.InvalidateRequerySuggested (beginner). REFERENCE SOLUTION.
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

using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Counts how many times <paramref name="command"/>'s CanExecuteChanged actually
/// notifies this observer.
/// </summary>
public sealed class Ex020_RequeryObserver : IDisposable
{
    private readonly ICommand _command;
    private readonly EventHandler? _handler;

    /// <summary>How many notifications this observer has received. Starts at 0.</summary>
    public int Count { get; private set; }

    public Ex020_RequeryObserver(ICommand command)
    {
        _command = command;

        // Kept in a field, not just subscribed as an inline lambda: RequerySuggested
        // holds its handlers weakly, so a delegate with no other strong reference
        // could be collected before it fires again.
        _handler = (_, _) => Count++;
        _command.CanExecuteChanged += _handler;
    }

    public void Dispose()
    {
        _command.CanExecuteChanged -= _handler;
    }
}
