// Exercise 012 - Stack Panel Spacing (beginner).
// Goal:   Stack children along one axis with a gap between them.
// Drills: StackPanel.Orientation and Spacing, how DesiredSize accumulates along the
//         stacking axis and takes the maximum across it, and that n children have n-1 gaps.
// Passes: dotnet test --filter FullyQualifiedName~Ex012_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex012_StackPanelSpacing
{
    /// <summary>
    /// A StackPanel stacking <paramref name="children"/> along
    /// <paramref name="orientation"/> with <paramref name="spacing"/> between neighbours -
    /// between them only, not around them.
    /// </summary>
    public static StackPanel Create(Orientation orientation, double spacing, params FrameworkElement[] children) =>
        // TODO: create the panel, set Orientation and Spacing, add every child.
        throw new NotImplementedException("TODO: Ex012 - build the spaced stack");
}
