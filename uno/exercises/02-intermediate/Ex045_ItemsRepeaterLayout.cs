// Exercise 045 - Items Repeater Layout (intermediate).
// Goal:   Write a layout for ItemsRepeater instead of a Panel.
// Drills: NonVirtualizingLayout with its MeasureOverride/ArrangeOverride pair, the
//         LayoutContext as the only way to reach the children, and swapping a repeater's
//         layout at runtime.
// Passes: dotnet test --filter FullyQualifiedName~Ex045_
//
// The same two verbs as a Panel (ex039/ex040), with one difference that matters: a Layout
// owns no children. It asks the context for them, which is what lets the same layout object
// be reused, and what makes the virtualising variant possible at all - there the context
// hands out only the elements inside the realisation window.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Lays elements out in a fixed number of equal-width columns, filling row by row: element
/// 0 top-left, then rightwards, wrapping after <see cref="Columns"/> elements. Every cell is
/// <see cref="RowHeight"/> tall.
/// </summary>
public sealed class Ex045_ItemsRepeaterLayout : NonVirtualizingLayout
{
    /// <summary>How many columns to fill before wrapping. At least 1.</summary>
    public int Columns { get; set; } = 2;

    /// <summary>The height of every row.</summary>
    public double RowHeight { get; set; } = 20;

    protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize) =>
        // TODO: measure every child in context.Children with one cell's size - the available
        // width divided by Columns, and RowHeight - then return the full available width and
        // the height of however many rows the children need.
        //
        // Rows are the ceiling of count / Columns, not count / Columns.
        throw new NotImplementedException("TODO: Ex045 - measure the grid of cells");

    protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize) =>
        // TODO: arrange child n into its cell: column n % Columns, row n / Columns. Return
        // finalSize.
        throw new NotImplementedException("TODO: Ex045 - arrange the grid of cells");

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/> using this layout and a
    /// template that renders each item as a Border 10 wide and 10 high, aligned to the top
    /// left of its cell - otherwise the Stretch default centres it and the element's offset
    /// stops being the cell's origin.
    /// </summary>
    public static ItemsRepeater CreateRepeater(object items, Ex045_ItemsRepeaterLayout layout) =>
        throw new NotImplementedException("TODO: Ex045 - build the repeater with this layout");
}
