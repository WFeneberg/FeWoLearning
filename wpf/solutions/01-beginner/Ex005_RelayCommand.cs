// Exercise 005 - Relay command (beginner). REFERENCE SOLUTION.
// Goal:   Replace a Click handler with an ICommand a view model owns, so the button's
//         enabled state stops being something the code-behind toggles by hand.
// Drills: ICommand.Execute/CanExecute, delegating both to constructor callbacks, and
//         routing CanExecuteChanged through CommandManager.RequerySuggested so WPF
//         re-asks on its own.

using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public sealed class Ex005_RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <param name="execute">What the command does. Required.</param>
    /// <param name="canExecute">When it may run. Null means "always".</param>
    public Ex005_RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Forwarded to the CommandManager, which re-asks CanExecute after input, focus
    /// changes and InvalidateRequerySuggested() - so neither this class nor the view
    /// model has to know when the answer might have changed.
    /// </summary>
    /// <remarks>
    /// RequerySuggested stores its handlers weakly, which is what makes forwarding safe:
    /// a command never keeps a subscriber alive. The other side of that bargain is that
    /// a subscriber must keep its own delegate alive.
    /// </remarks>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>True when the command may run. No predicate means always.</summary>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <summary>Runs the command.</summary>
    public void Execute(object? parameter) => _execute(parameter);
}
