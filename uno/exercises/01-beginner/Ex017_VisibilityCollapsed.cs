// Exercise 017 - Visibility And Opacity (beginner).
// Goal:   Hide an element two different ways and understand which one costs layout space.
// Drills: Visibility.Collapsed leaving the layout entirely (its DesiredSize is zero, and
//         its StackPanel gap disappears with it) against Opacity 0, which is invisible but
//         still measured, arranged and hit-testable.
// Passes: dotnet test --filter FullyQualifiedName~Ex017_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex017_VisibilityCollapsed
{
    /// <summary>
    /// Makes <paramref name="element"/> invisible.
    /// </summary>
    /// <param name="keepSpace">
    /// When true the element keeps its place in the layout and only stops being drawn;
    /// when false it leaves the layout as if it were not there.
    /// </param>
    public static void Hide(FrameworkElement element, bool keepSpace) =>
        // TODO: pick the right property for each case, and change only that one - the test
        // checks that the other is left alone.
        throw new NotImplementedException("TODO: Ex017 - hide the element the requested way");
}
