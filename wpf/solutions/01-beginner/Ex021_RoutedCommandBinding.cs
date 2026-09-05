// Exercise 021 - Routed command binding (beginner). REFERENCE SOLUTION.
// Goal:   Wire ApplicationCommands.Save to a handler through a CommandBinding registered
//         on an ancestor element, then invoke it with an explicit target so the call has
//         to route up the tree to find that binding - no focus and no window required.
// Drills: RoutedCommand (ApplicationCommands.Save is one), CommandBinding (Executed and
//         CanExecute registered on an element's CommandBindings collection), and
//         ApplicationCommands as the predefined command library used here. Measured, not
//         assumed: a RoutedCommand's Execute(parameter, target) is not "just" ICommand's
//         Execute - unlike Ex005's hand-rolled RelayCommand, where the caller must check
//         CanExecute itself before calling Execute, RoutedCommand.Execute raises CanExecute
//         through the very same CommandBinding first and only proceeds to Executed if that
//         returns true.

using System.Windows;
using System.Windows.Input;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex021_RoutedCommandBinding
{
    /// <summary>
    /// Registers a CommandBinding for <see cref="ApplicationCommands.Save"/> on
    /// <paramref name="owner"/>, wiring <paramref name="executed"/> and
    /// <paramref name="canExecute"/> to it.
    /// </summary>
    public static void Wire(UIElement owner, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
    {
        owner.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, executed, canExecute));
    }
}
