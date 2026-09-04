// Exercise 010 - Grid Placement (beginner).
// Goal:   Build a two-by-two Grid in code and put children in specific cells.
// Drills: RowDefinitions/ColumnDefinitions, GridLength (absolute, Auto, star),
//         Grid.SetRow/Grid.SetColumn, and what happens to a child that says nothing.
// Passes: dotnet test --filter FullyQualifiedName~Ex010_

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
    public static Grid CreateBadge(FrameworkElement icon, FrameworkElement header, FrameworkElement body)
    {
        var grid = new Grid();

        // Auto asks the children how tall they are; star divides what is left over. The
        // order of the definitions is the order of the indices - there are no names.
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Children are added to the Grid, and *then* say where they belong. A child that
        // never says lands in row 0, column 0 - which is why forgetting a Grid.SetRow
        // shows up as two elements drawn on top of each other rather than as an error.
        grid.Children.Add(icon);
        Grid.SetRow(icon, 0);
        Grid.SetColumn(icon, 0);

        grid.Children.Add(header);
        Grid.SetRow(header, 0);
        Grid.SetColumn(header, 1);

        grid.Children.Add(body);
        Grid.SetRow(body, 1);
        Grid.SetColumn(body, 1);

        return grid;
    }
}
