// Exercise 031 - SharedSizeGroup (beginner). REFERENCE SOLUTION.
// Goal:   Synchronize a row's height across two otherwise-unrelated Grids the way a form
//         lays out matching label rows in separate columns: Grid.IsSharedSizeScope=true on
//         a common ancestor, plus the same SharedSizeGroup name on a RowDefinition in each
//         descendant Grid, makes both rows measure to the size of whichever one needs the
//         most - even though the two Grids share no Grid.RowDefinitions collection at all.
// Drills: Grid.IsSharedSizeScope (must be set on an ancestor of every participating Grid,
//         not on the participating Grids themselves) and RowDefinition.SharedSizeGroup (the
//         string name that ties rows together across Grids). Measured directly: leaving
//         either half off - the ancestor flag or the matching group name - collapses back to
//         each row sizing independently, which is exactly what BuildTwoRowsSharingSize's
//         applySharedSizeScope=false path proves. Both RowDefinitions here are explicitly
//         GridUnitType.Auto, not left at ColumnDefinition/RowDefinition's own unassigned
//         default (also Star(1), same trap as row 029) - and deliberately NOT Star, because
//         SharedSizeGroup only equalizes Auto and pixel sizes; measured directly, giving a
//         Star-sized RowDefinition a SharedSizeGroup does not make it merely inert, it breaks
//         its ordinary Star measurement outright (collapses to zero), so this row's evidence
//         is built entirely on Auto rows.

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex031_SharedSizeGroup
{
    /// <summary>
    /// Builds an outer Grid with two columns, each hosting its own nested Grid with a single
    /// Auto-height row tagged <paramref name="sharedSizeGroupName"/>: the left nested Grid's
    /// row holds a Border of height <paramref name="leftContentHeight"/>, the right nested
    /// Grid's row holds one of height <paramref name="rightContentHeight"/>. When
    /// <paramref name="applySharedSizeScope"/> is true, the outer Grid also gets
    /// Grid.IsSharedSizeScope=true, so the two rows - despite belonging to two different
    /// Grids - measure to the same (larger) height. When false, the group name is still set
    /// on both rows, but with no ancestor scope to register them into, each row sizes to its
    /// own content independently.
    /// </summary>
    public static Grid BuildTwoRowsSharingSize(
        string sharedSizeGroupName,
        double leftContentHeight,
        double rightContentHeight,
        bool applySharedSizeScope)
    {
        var outer = new Grid();
        if (applySharedSizeScope) Grid.SetIsSharedSizeScope(outer, true);
        outer.ColumnDefinitions.Add(new ColumnDefinition());
        outer.ColumnDefinitions.Add(new ColumnDefinition());

        var left = new Grid();
        left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto), SharedSizeGroup = sharedSizeGroupName });
        var leftContent = new Border { Height = leftContentHeight };
        Grid.SetRow(leftContent, 0);
        left.Children.Add(leftContent);

        var right = new Grid();
        right.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto), SharedSizeGroup = sharedSizeGroupName });
        var rightContent = new Border { Height = rightContentHeight };
        Grid.SetRow(rightContent, 0);
        right.Children.Add(rightContent);

        Grid.SetColumn(left, 0);
        Grid.SetColumn(right, 1);
        outer.Children.Add(left);
        outer.Children.Add(right);
        return outer;
    }
}
