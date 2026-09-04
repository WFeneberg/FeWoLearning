// Exercise 011 - Star Sizing (beginner).
// Goal:   Divide a row three ways: content-sized, then the rest by weight.
// Drills: GridLength with GridUnitType.Auto/Pixel/Star, star as a *weight* rather than a
//         percentage, and what the children's ActualWidth becomes after arrange.
// Passes: dotnet test --filter FullyQualifiedName~Ex011_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex011_StarSizing
{
    /// <summary>
    /// A single row of three columns:
    /// <list type="number">
    ///   <item><paramref name="icon"/> in an Auto column - as wide as it asks to be,</item>
    ///   <item><paramref name="main"/> in a column of weight 2,</item>
    ///   <item><paramref name="aside"/> in a column of weight 1.</item>
    /// </list>
    /// So main always gets twice the leftover width of aside, whatever is left after the
    /// icon has taken what it needs.
    /// </summary>
    public static Grid CreateRatioRow(FrameworkElement icon, FrameworkElement main, FrameworkElement aside) =>
        // TODO: build the Grid with those three columns and put one child in each.
        throw new NotImplementedException("TODO: Ex011 - build the auto/2-star/1-star row");
}
