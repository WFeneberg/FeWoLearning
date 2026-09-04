// Exercise 013 - Margin And Padding (beginner).
// Goal:   Put space around a child from both sides of the relationship.
// Drills: Thickness(left, top, right, bottom) - in that order - and the difference between
//         a child's Margin and its parent's Padding.
// Passes: dotnet test --filter FullyQualifiedName~Ex013_
//
// Margin belongs to the child and travels with it into any parent. Padding belongs to the
// container and applies to whatever it happens to hold. Both end up in the measurement,
// which is why a "why is this 16 pixels too wide" bug can come from either end.

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
    public static Border CreateCard(FrameworkElement content) =>
        // TODO: create the Border, set its Padding, set the content's Margin, and make the
        // content its Child.
        throw new NotImplementedException("TODO: Ex013 - wrap the content in a padded card");
}
