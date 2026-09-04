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
    public static double ResolveWidth(double? width, double? minWidth, double? maxWidth, double available) =>
        // TODO: create the Border with a fixed Height of 10, apply only the constraints that
        // are not null, run a full measure and arrange over (available, 10), and return
        // ActualWidth.
        throw new NotImplementedException("TODO: Ex058 - resolve the effective width");
}
