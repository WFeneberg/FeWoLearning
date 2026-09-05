// Exercise 028 - The Measure/Arrange contract (beginner). REFERENCE SOLUTION.
// Goal:   See what a FrameworkElement actually receives and reports at each half of a
//         layout pass. MeasureOverride gets a constraint already reduced by Margin and
//         answers with a size Margin then gets added back onto to become DesiredSize.
//         ArrangeOverride gets a finalSize also reduced by Margin, and whatever it returns
//         becomes RenderSize verbatim - which does NOT include Margin. DesiredSize and
//         RenderSize can legitimately disagree; that disagreement is the whole subject of
//         this row, not a bug to paper over. This stays a single leaf element with no
//         children of its own - a Panel that lays out several children by a custom algorithm
//         is row 062's subject, not this one.
// Drills: MeasureOverride(Size constraint) and ArrangeOverride(Size finalSize) - the two
//         layout-pass hooks a leaf FrameworkElement overrides - and DesiredSize versus
//         RenderSize/ActualWidth/ActualHeight: DesiredSize includes Margin, RenderSize does
//         not.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// A leaf element that always wants to be <see cref="NaturalSize"/>, regardless of how
/// little space is on offer, and that records what each layout hook actually received - so a
/// test can inspect the contract directly instead of only the geometry it produces.
/// </summary>
public class Ex028_MeasureArrangeElement : FrameworkElement
{
    /// <summary>What this element wants to be. Set by the test before layout.</summary>
    public Size NaturalSize { get; set; } = new(40, 20);

    /// <summary>The constraint MeasureOverride actually received.</summary>
    public Size LastMeasureConstraint { get; private set; }

    /// <summary>The finalSize ArrangeOverride actually received.</summary>
    public Size LastArrangeBounds { get; private set; }

    protected override Size MeasureOverride(Size constraint)
    {
        LastMeasureConstraint = constraint;
        return NaturalSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        LastArrangeBounds = finalSize;
        return finalSize;
    }
}
