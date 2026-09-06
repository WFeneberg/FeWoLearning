// Exercise 062 - A real custom Panel (intermediate). REFERENCE SOLUTION.
// Goal:   Row 028 stayed a single leaf FrameworkElement with no children of its own precisely so
//         this row could own a genuine Panel: enumerating InternalChildren, measuring each one,
//         and arranging several of them by an algorithm the panel itself decides - not the single
//         override/return pair row 028 already covered. This one stacks children vertically at
//         their own natural width (no stretching), each positioned directly below the last.
// Drills: Panel.InternalChildren, calling Measure on each child with a constraint the panel
//         chooses (width taken from availableSize, height left unconstrained, since a vertical
//         stack never runs out of height to offer), summing the children's DesiredSize into the
//         panel's own returned size, and arranging each child at a running Y offset built from
//         the PREVIOUS children's actual DesiredSize - not a fixed increment and not everyone at
//         (0,0). Also: ArrangeOverride's return is not clamped to finalSize the way DesiredSize is
//         clamped to Measure's constraint (see README "Layout and sizing") - this panel returns
//         its OWN natural stacked size from ArrangeOverride, not finalSize, and RenderSize follows
//         that verbatim even when finalSize was much larger.

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Stacks its children vertically, each at its own natural (unstretched) width, one directly
/// below the previous. A real Panel subclass, not a leaf FrameworkElement - see the Goal comment
/// above for why row 028 could not already be this row.
/// </summary>
public class Ex062_StackingPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (UIElement child in InternalChildren)
        {
            child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double width = 0;
        double runningY = 0;

        foreach (UIElement child in InternalChildren)
        {
            var desired = child.DesiredSize;
            child.Arrange(new Rect(0, runningY, desired.Width, desired.Height));
            width = Math.Max(width, desired.Width);
            runningY += desired.Height;
        }

        return new Size(width, runningY);
    }
}
