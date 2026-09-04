// Exercise 029 - Event Handlers (beginner).
// Goal:   Wire a control's event to a change somewhere else in the tree.
// Drills: ButtonBase.Click, RoutedEventArgs and its sender, and reading the state back out
//         of the tree rather than out of a variable the handler happens to keep.
// Passes: dotnet test --filter FullyQualifiedName~Ex029_
//
// The test presses the button through its automation peer - the same path a screen reader
// takes. There is no synthetic mouse anywhere, and a handler that only reacts to a pointer
// would never be reachable by assistive technology either.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex029_EventHandlers
{
    /// <summary>
    /// A StackPanel holding a Button named "Increment" (Content "+") and a TextBlock named
    /// "Count" showing the number of clicks so far, starting at "0". Each click raises the
    /// number the TextBlock shows.
    /// </summary>
    public static StackPanel CreateCounter() =>
        // TODO: build the two elements, keep the count, and subscribe to Click so the
        // TextBlock follows it. The count belongs to this panel, not to a static field -
        // two counters must not share it.
        throw new NotImplementedException("TODO: Ex029 - wire the button to the label");
}
