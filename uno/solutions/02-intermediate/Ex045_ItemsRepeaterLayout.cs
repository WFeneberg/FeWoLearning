// Exercise 045 - Items Repeater Layout (intermediate).
// Goal:   Write a layout for ItemsRepeater instead of a Panel.
// Drills: NonVirtualizingLayout with its MeasureOverride/ArrangeOverride pair, the
//         LayoutContext as the only way to reach the children, and swapping a repeater's
//         layout at runtime.
// Passes: dotnet test --filter FullyQualifiedName~Ex045_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Lays elements out in a fixed number of equal-width columns, filling row by row: element
/// 0 top-left, then rightwards, wrapping after <see cref="Columns"/> elements. Every cell is
/// <see cref="RowHeight"/> tall.
/// </summary>
public sealed class Ex045_ItemsRepeaterLayout : NonVirtualizingLayout
{
    private static readonly DataTemplate CellTemplate = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <Border Width="10" Height="10" HorizontalAlignment="Left" VerticalAlignment="Top" />
        </DataTemplate>
        """);

    /// <summary>How many columns to fill before wrapping. At least 1.</summary>
    public int Columns { get; set; } = 2;

    /// <summary>The height of every row.</summary>
    public double RowHeight { get; set; } = 20;

    protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
    {
        var columns = Math.Max(1, Columns);
        var columnWidth = availableSize.Width / columns;

        // context.Children, not a Children property of our own: a Layout is a strategy
        // object with no tree of its own, which is what lets one instance serve several
        // repeaters - and what makes the virtualising variant possible, where the context
        // hands out only the realised window.
        foreach (var child in context.Children)
        {
            child.Measure(new Size(columnWidth, RowHeight));
        }

        // Ceiling, not integer division: five items in two columns is three rows.
        var rows = (context.Children.Count + columns - 1) / columns;

        return new Size(availableSize.Width, rows * RowHeight);
    }

    protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
    {
        var columns = Math.Max(1, Columns);
        var columnWidth = finalSize.Width / columns;

        for (var index = 0; index < context.Children.Count; index++)
        {
            var column = index % columns;
            var row = index / columns;

            context.Children[index].Arrange(
                new Rect(column * columnWidth, row * RowHeight, columnWidth, RowHeight));
        }

        return finalSize;
    }

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/> using this layout and a
    /// template that renders each item as a Border 10 wide and 10 high, aligned to the top
    /// left of its cell - otherwise the Stretch default centres it and the element's offset
    /// stops being the cell's origin.
    /// </summary>
    public static ItemsRepeater CreateRepeater(object items, Ex045_ItemsRepeaterLayout layout) => new()
    {
        ItemsSource = items,
        ItemTemplate = CellTemplate,
        Layout = layout,
    };
}
