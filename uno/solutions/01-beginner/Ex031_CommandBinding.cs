// Exercise 031 - Command Binding (beginner).
// Goal:   Move "can this be done, and do it" out of an event handler and into an object.
// Drills: ICommand with CanExecute and CanExecuteChanged, ButtonBase.Command driving
//         IsEnabled, and a disabled button refusing to execute.
// Passes: dotnet test --filter FullyQualifiedName~Ex031_

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

    public bool CanExecute(object? parameter) => _canExecute();

    public void Execute(object? parameter)
    {
        // Asked again here on purpose. A disabled button will not call this, but a keyboard
        // accelerator, a test, or another view model will - and a command that trusts its
        // callers is a command that runs when it should not.
        if (!CanExecute(parameter))
        {
            return;
        }

        _execute(parameter);
    }

    /// <summary>
    /// Announces that the answer may have changed. Nothing else re-queries a command.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// A Button bound to <paramref name="command"/>, so its IsEnabled follows
    /// <see cref="CanExecute"/> without anybody setting it.
    /// </summary>
    public static Button CreateBoundButton(ICommand command) => new()
    {
        // Setting Command is the whole wiring: ButtonBase subscribes to CanExecuteChanged,
        // asks CanExecute, and writes IsEnabled itself. Touching IsEnabled by hand from
        // here would fight it.
        Command = command,
    };
}
