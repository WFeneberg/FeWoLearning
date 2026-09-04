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
    public IReadOnlyList<double> ComputeWidths(double availableWidth)
    {
        var unbounded = new Size(double.PositiveInfinity, RowHeight);
        var fixedWidth = 0d;
        var totalFactors = 0;

        // First pass: what everybody wants, and how much width is already claimed. The
        // grow factors cannot be applied yet - the leftover is not known until this loop
        // has finished, which is exactly why the pass is split.
        foreach (var child in Children)
        {
            child.Measure(unbounded);

            var grow = GetGrow(child);

            if (grow <= 0)
            {
                fixedWidth += child.DesiredSize.Width;
            }
            else
            {
                totalFactors += grow;
            }
        }

        // Never negative: a negative width throws inside Measure.
        var leftover = Math.Max(0, availableWidth - fixedWidth);

        return Children
            .Select(child => GetGrow(child) is var grow && grow > 0
                ? (totalFactors == 0 ? 0 : leftover * grow / totalFactors)
                : child.DesiredSize.Width)
            .ToList();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var widths = ComputeWidths(availableSize.Width);

        for (var index = 0; index < Children.Count; index++)
        {
            Children[index].Measure(new Size(widths[index], RowHeight));
        }

        return new Size(availableSize.Width, RowHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // The same computation, against the final width. Reading the children's DesiredSize
        // instead would ignore the shares: an explicit Width reports itself, and a child
        // without one reports nothing.
        var widths = ComputeWidths(finalSize.Width);
        var x = 0d;

        for (var index = 0; index < Children.Count; index++)
        {
            Children[index].Arrange(new Rect(x, 0, widths[index], RowHeight));
            x += widths[index];
        }

        return finalSize;
    }
}
