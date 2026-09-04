// Exercise 005 - Relay command (beginner).
// Goal:   Replace a Click handler with an ICommand a view model owns, so the button's
//         enabled state stops being something the code-behind toggles by hand.
// Drills: ICommand.Execute/CanExecute, delegating both to constructor callbacks, and
//         routing CanExecuteChanged through CommandManager.RequerySuggested so WPF
//         re-asks on its own.
// Passes: dotnet test --filter FullyQualifiedName~Ex005_

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

    // TODO: implement CanExecuteChanged as an event whose add/remove forward to
    // CommandManager.RequerySuggested. That is what makes WPF re-ask CanExecute after
    // input, focus changes and CommandManager.InvalidateRequerySuggested() - without
    // the view model raising anything itself.
    //
    // Note: RequerySuggested holds its handlers *weakly*, which is exactly why
    // forwarding is correct here and a hand-rolled event field is not.
    public event EventHandler? CanExecuteChanged
    {
        add => throw new NotImplementedException("TODO: Ex005 - forward the subscription to CommandManager.RequerySuggested");
        remove => throw new NotImplementedException("TODO: Ex005 - forward the unsubscription to CommandManager.RequerySuggested");
    }

    /// <summary>True when the command may run. No predicate means always.</summary>
    public bool CanExecute(object? parameter)
        // TODO: ask _canExecute, and treat a null predicate as "always true".
        => throw new NotImplementedException("TODO: Ex005 - implement CanExecute");

    /// <summary>Runs the command.</summary>
    public void Execute(object? parameter)
        // TODO: invoke _execute with the parameter.
        => throw new NotImplementedException("TODO: Ex005 - implement Execute");
}
