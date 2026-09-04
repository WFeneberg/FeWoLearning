// Exercise 097 - Flex Layout Engine (expert).
// Goal:   Distribute leftover space by weight, the way flexbox does.
// Drills: a two-pass measure (fixed children first, then the leftover by grow factor), an
//         attached property carrying per-child layout data, and a layout that stays correct
//         when there is no space left to give.
// Passes: dotnet test --filter FullyQualifiedName~Ex097_
//
// The interesting part is not the arithmetic, it is the ordering: a child's grow factor
// cannot be applied until every non-growing child has been measured, so the pass has to be
// split. Do it in one pass and the first child gets everything.
//
// And the widths cannot be carried from measure to arrange in the children's DesiredSize:
// a child with an explicit Width reports that (ex058), and one without reports nothing.
// Both passes therefore run the same computation - the ex041 rule, met again.
//
// The per-child data lives in an attached property, which is how Grid.Row, Canvas.Left and
// every real layout does it - the child needs no base class and no interface.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Expert;

public partial class Ex097_FlexLayoutEngine : Panel
{
    /// <summary>
    /// How many shares of the leftover space a child claims. 0 - the default - means the
    /// child keeps its own desired width.
    /// </summary>
    public static readonly DependencyProperty GrowProperty =
        DependencyProperty.RegisterAttached(
            "Grow",
            typeof(int),
            typeof(Ex097_FlexLayoutEngine),
            new PropertyMetadata(0));

    public static int GetGrow(DependencyObject element) => (int)element.GetValue(GrowProperty);

    public static void SetGrow(DependencyObject element, int value) => element.SetValue(GrowProperty, value);

    /// <summary>The height every child is given.</summary>
    public double RowHeight { get; set; } = 20;

    /// <summary>
    /// Measures the children left to right: a child with no grow factor keeps its desired
    /// width, and the remaining width is split between the growing children in proportion
    /// to their factors. Returns the full available width and <see cref="RowHeight"/>.
    /// </summary>
    /// <summary>
    /// The width each child gets, in order, for a panel <paramref name="availableWidth"/>
    /// wide.
    /// </summary>
    public IReadOnlyList<double> ComputeWidths(double availableWidth) =>
        // TODO: two passes over the children.
        //
        // First: measure each child with an unbounded width and RowHeight, and add up the
        // desired widths of the ones whose grow factor is 0 - those keep their own width.
        //
        // Then: the leftover is availableWidth minus that total, never below zero, and each
        // growing child gets leftover * factor / totalFactors.
        throw new NotImplementedException("TODO: Ex097 - work out the widths");

    protected override Size MeasureOverride(Size availableSize) =>
        // TODO: compute the widths, measure each child with its own, and return the full
        // available width and RowHeight.
        throw new NotImplementedException("TODO: Ex097 - measure with the computed widths");

    protected override Size ArrangeOverride(Size finalSize) =>
        // TODO: compute the widths again against finalSize and place the children side by
        // side. Reading DesiredSize instead would ignore the shares entirely.
        throw new NotImplementedException("TODO: Ex097 - lay the children out in a row");
}
