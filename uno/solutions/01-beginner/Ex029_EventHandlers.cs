// Exercise 029 - Event Handlers (beginner).
// Goal:   Wire a control's event to a change somewhere else in the tree.
// Drills: ButtonBase.Click, RoutedEventArgs and its sender, and reading the state back out
//         of the tree rather than out of a variable the handler happens to keep.
// Passes: dotnet test --filter FullyQualifiedName~Ex029_

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex029_EventHandlers
{
    /// <summary>
    /// A StackPanel holding a Button named "Increment" (Content "+") and a TextBlock named
    /// "Count" showing the number of clicks so far, starting at "0". Each click raises the
    /// number the TextBlock shows.
    /// </summary>
    public static StackPanel CreateCounter()
    {
        var increment = new Button { Name = "Increment", Content = "+" };
        var count = new TextBlock { Name = "Count", Text = "0" };

        // A local, captured by the handler: one per call, so two counters count separately.
        // A static field here would be shared by every counter in the app - the classic
        // version of this bug, and invisible until the second one appears on screen.
        var clicks = 0;

        increment.Click += (_, _) =>
        {
            clicks++;
            count.Text = clicks.ToString();
        };

        var panel = new StackPanel();
        panel.Children.Add(increment);
        panel.Children.Add(count);
        return panel;
    }
}
