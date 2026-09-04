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
    public static void Hide(FrameworkElement element, bool keepSpace)
    {
        if (keepSpace)
        {
            // Still measured, still arranged, still hit-testable - which is occasionally
            // the point, and occasionally the bug: an invisible element can swallow taps.
            element.Opacity = 0;
            return;
        }

        // Collapsed elements are skipped by the measure pass, so they contribute no size
        // and no StackPanel gap. There is no Visibility.Hidden in WinUI - that is WPF.
        element.Visibility = Visibility.Collapsed;
    }
}
