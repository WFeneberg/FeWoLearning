// Exercise 039 - Custom Panel Measure (intermediate).
// Goal:   Write the measure half of a layout panel.
// Drills: MeasureOverride, the constraint you hand each child, reading DesiredSize back,
//         and surviving an infinite available size.
// Passes: dotnet test --filter FullyQualifiedName~Ex039_
//
// The constraint you pass down is a promise, not a suggestion: a child measured with 100
// will report a DesiredSize clamped to 100 and then be arranged into whatever you give it.
// Measuring children with infinity and arranging them into a finite slot is how content
// ends up silently clipped.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Lays children out in equal-width columns: each child gets exactly one nth of the
/// available width, and the panel is as tall as its tallest child.
/// </summary>
public partial class Ex039_CustomPanelMeasure : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
        {
            return new Size(0, 0);
        }

        // Infinity divided by the child count is still infinity, and an infinite
        // DesiredSize takes the layout pass down. When the width is unbounded there are no
        // columns to divide, so measure the children as they are and add them up.
        var unbounded = double.IsInfinity(availableSize.Width);
        var columnWidth = unbounded ? double.PositiveInfinity : availableSize.Width / Children.Count;

        var width = 0d;
        var height = 0d;

        foreach (var child in Children)
        {
            // The constraint is a promise: the child's DesiredSize comes back clamped to
            // it, which is what lets the panel trust the number below.
            child.Measure(new Size(columnWidth, availableSize.Height));

            width += child.DesiredSize.Width;
            height = Math.Max(height, child.DesiredSize.Height);
        }

        // Bounded: the panel fills its columns, so it wants everything it was offered.
        // Unbounded: it wants exactly what its children turned out to need.
        return new Size(unbounded ? width : availableSize.Width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Given: the arrange half is Ex040's subject.
        var columnWidth = Children.Count == 0 ? 0 : finalSize.Width / Children.Count;
        var x = 0d;

        foreach (var child in Children)
        {
            child.Arrange(new Rect(x, 0, columnWidth, finalSize.Height));
            x += columnWidth;
        }

        return finalSize;
    }
}
