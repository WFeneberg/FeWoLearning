// Exercise 058 - Size Constraints (intermediate).
// Goal:   Work out what an element's width ends up being when three properties disagree.
// Drills: MinWidth/MaxWidth against Width, the order the framework clamps them in, and the
//         fact that a minimum can push an element past the space it was offered.
// Passes: dotnet test --filter FullyQualifiedName~Ex058_
//
// The effective width is max(MinWidth, min(MaxWidth, Width)) - so the minimum has the last
// word, over the maximum and over an explicit Width alike. Every "why is this element
// wider than its container" report is that last clamp.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex058_SizeConstraints
{
    /// <summary>
    /// Builds a Border with only the constraints that were given, lays it out in
    /// <paramref name="available"/> pixels of width, and reports the width it ended up with.
    /// </summary>
    /// <remarks>
    /// A null argument means "not set", and not set is not the same as zero: an unset Width
    /// is NaN, an unset MinWidth is 0 and an unset MaxWidth is positive infinity. Assigning
    /// those defaults by hand would change the answer.
    /// </remarks>
    public static double ResolveWidth(double? width, double? minWidth, double? maxWidth, double available)
    {
        var element = new Border { Height = 10 };

        // Each one assigned only if it was given. Writing the "defaults" instead - 0 for a
        // minimum, 0 for a maximum - would collapse the element and look like a measure
        // bug rather than the assignment it is.
        if (width is not null)
        {
            element.Width = width.Value;
        }

        if (minWidth is not null)
        {
            element.MinWidth = minWidth.Value;
        }

        if (maxWidth is not null)
        {
            element.MaxWidth = maxWidth.Value;
        }

        // Both passes: ActualWidth is what arrange handed over, and the clamping happens
        // in measure - reading it before either would report 0.
        element.Measure(new Size(available, 10));
        element.Arrange(new Rect(0, 0, available, 10));

        return element.ActualWidth;
    }
}
