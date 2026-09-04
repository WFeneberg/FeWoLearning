// Exercise 072 - Virtualizing Layout Window (advanced).
// Goal:   Realise only the elements a viewport can actually show.
// Drills: VirtualizingLayout, VirtualizingLayoutContext.RealizationRect and ItemCount,
//         GetOrCreateElementAt as the only way to obtain an element, and the index range
//         that follows from a rect.
// Passes: dotnet test --filter FullyQualifiedName~Ex072_
//
// The difference to a NonVirtualizingLayout is the whole point: the context does not hand
// you a Children collection, it hands you a rect and a count, and you ask for the elements
// you need by index. A layout that asks for all of them compiles, runs, and destroys the
// reason virtualisation exists.
//
// The harness has no viewport, so the realisation rect it reports is empty - see
// uno/README.md. That is why the assertions are about the index range this layout *derives*
// from a rect, which is the part worth getting right anyway.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A fixed-height row layout that realises only the rows inside the realisation rect.
/// </summary>
public sealed class Ex072_VirtualizingLayoutWindow : VirtualizingLayout
{
    /// <summary>The height of every row.</summary>
    public double RowHeight { get; set; } = 20;

    /// <summary>The item indices this layout asked the context for, in order.</summary>
    public List<int> RequestedIndices { get; } = [];

    /// <summary>The realisation rect of the last measure pass.</summary>
    public Rect LastRealizationRect { get; private set; }

    /// <summary>
    /// The half-open index range that <paramref name="rect"/> covers, clamped to
    /// <paramref name="itemCount"/>: everything whose row overlaps the rect vertically.
    /// An empty rect covers nothing.
    /// </summary>
    public (int First, int Count) RangeFor(Rect rect, int itemCount)
    {
        if (rect.Height <= 0 || itemCount <= 0)
        {
            return (0, 0);
        }

        // Floor the top and ceiling the bottom: a row the rect only partly covers still
        // has to exist, or the viewport shows a gap where half a row should be.
        var first = Math.Max(0, (int)Math.Floor(rect.Top / RowHeight));
        var last = Math.Min(itemCount, (int)Math.Ceiling(rect.Bottom / RowHeight));

        return first >= last ? (0, 0) : (first, last - first);
    }

    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        LastRealizationRect = context.RealizationRect;

        var (first, count) = RangeFor(context.RealizationRect, context.ItemCount);

        for (var index = first; index < first + count; index++)
        {
            RequestedIndices.Add(index);

            // The only way to obtain an element. There is no Children collection here -
            // which is exactly what stops a virtualising layout from touching all 50,000
            // items by accident.
            context.GetOrCreateElementAt(index).Measure(new Size(availableSize.Width, RowHeight));
        }

        // The extent: every row, realised or not. Reporting the realised height instead
        // makes the scroll bar grow and shrink as you scroll - the classic symptom of a
        // layout that confuses the window with the whole.
        return new Size(availableSize.Width, context.ItemCount * RowHeight);
    }

    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        // Given: arrange each realised element into its row.
        var (first, count) = RangeFor(context.RealizationRect, context.ItemCount);

        for (var index = first; index < first + count; index++)
        {
            context.GetOrCreateElementAt(index)
                .Arrange(new Rect(0, index * RowHeight, finalSize.Width, RowHeight));
        }

        return finalSize;
    }

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/> using this layout, each
    /// item a Border 10 by 10 aligned top-left.
    /// </summary>
    public static ItemsRepeater CreateRepeater(object items, Ex072_VirtualizingLayoutWindow layout) => new()
    {
        ItemsSource = items,
        ItemTemplate = ItemTemplate,
        Layout = layout,
    };

    private static readonly DataTemplate ItemTemplate = (DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <Border Width="10" Height="10" HorizontalAlignment="Left" VerticalAlignment="Top" />
        </DataTemplate>
        """);
}
