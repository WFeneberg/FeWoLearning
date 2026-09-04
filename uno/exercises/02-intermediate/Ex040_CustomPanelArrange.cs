// Exercise 040 - Custom Panel Arrange (intermediate).
// Goal:   Write the arrange half of a layout panel, and decide where children really go.
// Drills: ArrangeOverride, the Rect you hand each child, the difference between the size
//         you return and the size you were given, and arranging a child outside the panel.
// Passes: dotnet test --filter FullyQualifiedName~Ex040_
//
// Arrange is not clamped for you: a Rect that reaches past the panel is honoured, the child
// is positioned there, and it is clipped only if something up the tree clips. That is how
// overlays and drop shadows are built - and how children go missing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Stacks children diagonally: child n sits at (n * Offset, n * Offset) at its own desired
/// size. The panel reports the bounding box it actually used.
/// </summary>
public partial class Ex040_CustomPanelArrange : Panel
{
    /// <summary>How far each child is shifted from the previous one, on both axes.</summary>
    public double Offset { get; set; } = 10;

    protected override Size MeasureOverride(Size availableSize)
    {
        // Given: children are measured unconstrained, because the diagonal decides the
        // layout rather than the available space.
        var unbounded = new Size(double.PositiveInfinity, double.PositiveInfinity);
        var width = 0d;
        var height = 0d;
        var index = 0;

        foreach (var child in Children)
        {
            child.Measure(unbounded);
            width = Math.Max(width, (index * Offset) + child.DesiredSize.Width);
            height = Math.Max(height, (index * Offset) + child.DesiredSize.Height);
            index++;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize) =>
        // TODO: arrange child n at (n * Offset, n * Offset), each at its own DesiredSize -
        // not at the panel's size, and not clamped to finalSize.
        //
        // Return the bounding box the children actually cover, which may be larger than
        // finalSize. Returning finalSize unchanged is the usual reflex and is wrong here:
        // the return value is what the panel reports as its arranged size.
        throw new NotImplementedException("TODO: Ex040 - arrange the children on the diagonal");
}
