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
    protected override Size MeasureOverride(Size availableSize) =>
        // TODO: measure every child with one nth of the available width and the full
        // available height, then return the size this panel needs: the whole available
        // width (it fills its columns) and the tallest child's desired height.
        //
        // Two cases the test checks: no children at all, and an infinite available width -
        // which is what a StackPanel or a ScrollViewer hands down. Dividing infinity by
        // three is still infinity, and returning it as a DesiredSize is a crash. Measure
        // the children unconstrained instead and add their widths up.
        throw new NotImplementedException("TODO: Ex039 - measure the columns");

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
