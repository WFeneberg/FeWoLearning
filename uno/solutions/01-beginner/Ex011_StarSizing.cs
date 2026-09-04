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
    public static Grid CreateRatioRow(FrameworkElement icon, FrameworkElement main, FrameworkElement aside)
    {
        var grid = new Grid();

        // Auto is resolved first, from the child's own DesiredSize. Whatever survives that
        // is what the star columns get to divide - which is why star is a weight: 2 and 1
        // mean "two thirds and one third of the remainder", not "66% and 33% of the Grid".
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        grid.Children.Add(icon);
        Grid.SetColumn(icon, 0);

        grid.Children.Add(main);
        Grid.SetColumn(main, 1);

        grid.Children.Add(aside);
        Grid.SetColumn(aside, 2);

        // No RowDefinitions: a Grid without them has one implicit row that fills the
        // height, so every child lands in row 0 without being told.
        return grid;
    }
}
