// Exercise 013 - Margin And Padding (beginner).
// Goal:   Put space around a child from both sides of the relationship.
// Drills: Thickness(left, top, right, bottom) - in that order - and the difference between
//         a child's Margin and its parent's Padding.
// Passes: dotnet test --filter FullyQualifiedName~Ex013_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex013_MarginPadding
{
    /// <summary>
    /// Wraps <paramref name="content"/> in a Border that pads itself
    /// 12 left / 8 top / 4 right / 2 bottom, and gives the content a uniform margin of 4.
    /// </summary>
    /// <remarks>
    /// The asymmetry is deliberate: a uniform thickness hides an argument-order mistake.
    /// </remarks>
    public static Border CreateCard(FrameworkElement content)
    {
        // Clockwise from the left, like CSS - but unlike CSS there is no shorthand, so a
        // transposed pair is a silent layout bug rather than a parse error.
        content.Margin = new Thickness(4);

        return new Border
        {
            Padding = new Thickness(12, 8, 4, 2),
            Child = content,
        };
    }
}
