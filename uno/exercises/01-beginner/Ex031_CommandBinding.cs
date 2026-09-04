// Exercise 031 - Command Binding (beginner).
// Goal:   Move "can this be done, and do it" out of an event handler and into an object.
// Drills: ICommand with CanExecute and CanExecuteChanged, ButtonBase.Command driving
//         IsEnabled, and a disabled button refusing to execute.
// Passes: dotnet test --filter FullyQualifiedName~Ex031_
//
// A Click handler can only live in code-behind. A command is a property, so it can come
// from a view model, be shared by a button and a menu item, and be tested without a UI.

using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>
/// A command whose executability is decided by a predicate the caller supplies, and which
/// can be told to re-ask.
/// </summary>
public sealed class Ex031_CommandBinding : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<bool> _canExecute;

    public Ex031_CommandBinding(Action<object?> execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>Raised to tell every bound control to call <see cref="CanExecute"/> again.</summary>
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        throw new NotImplementedException("TODO: Ex031 - answer from the predicate");

    public void Execute(object? parameter) =>
        // TODO: run the action - but only when the command can execute. A command is the
        // last line of defence: nothing guarantees the caller asked first.
        throw new NotImplementedException("TODO: Ex031 - execute if allowed");

    /// <summary>
    /// Announces that the answer may have changed. Nothing else re-queries a command.
    /// </summary>
    public void RaiseCanExecuteChanged() =>
        throw new NotImplementedException("TODO: Ex031 - raise the event");

    /// <summary>
    /// A Button bound to <paramref name="command"/>, so its IsEnabled follows
    /// <see cref="CanExecute"/> without anybody setting it.
    /// </summary>
    public static Button CreateBoundButton(ICommand command) =>
        throw new NotImplementedException("TODO: Ex031 - bind the command to a button");
}
