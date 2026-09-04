// Exercise 010 - Grid Placement (beginner).
// Goal:   Build a two-by-two Grid in code and put children in specific cells.
// Drills: RowDefinitions/ColumnDefinitions, GridLength (absolute, Auto, star),
//         Grid.SetRow/Grid.SetColumn, and what happens to a child that says nothing.
// Passes: dotnet test --filter FullyQualifiedName~Ex010_
//
// Grid.Row is the attached property from Ex003, seen from the consuming side.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex010_GridPlacement
{
    /// <summary>
    /// A badge layout: a 40-pixel icon column and a content column that takes the rest,
    /// over a header row that is as tall as it needs to be and a body row that fills.
    ///
    /// The Grid gets, in this order:
    /// <list type="number">
    ///   <item>the <paramref name="icon"/> in row 0, column 0,</item>
    ///   <item>the <paramref name="header"/> in row 0, column 1,</item>
    ///   <item>the <paramref name="body"/> in row 1, column 1.</item>
    /// </list>
    /// </summary>
    public static Grid CreateBadge(FrameworkElement icon, FrameworkElement header, FrameworkElement body) =>
        // TODO: create the Grid, give it two rows (Auto, then one star) and two columns
        // (40 pixels, then one star), add the three children and place them.
        throw new NotImplementedException("TODO: Ex010 - build and populate the badge grid");
}
