using System;
using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 073 - CustomLayoutPanel (advanced).
/// Goal:   Write a Panel that positions its own Children: a staircase, where each
///         child is offset from the one before it by Step on both axes. Where
///         ex072 sized a single control, this one is responsible for other
///         people's children.
/// Drills: Panel subclassing, iterating Children, Measure on each child before
///         reading DesiredSize, Arrange with an explicit Rect, reporting a
///         DesiredSize that accounts for the offsets.
/// Passes: dotnet test --filter FullyQualifiedName~Ex073_
///
/// Two things that are easy to get wrong and are both graded.
///
/// A child's DesiredSize is meaningless until you have measured it. Arranging
/// straight from Width/Height happens to work for a Border with both set and
/// falls apart for anything that sizes to content, so measure every child in
/// MeasureOverride even if you do not use the result there.
///
/// The panel's own DesiredSize has to include the staircase. With three 20x10
/// children and a Step of 5 the union is 30x20, not 20x10 and not 60x30: the last
/// child starts at 10,10 and is 20x10, so the far corner is at 30,20.
public class Ex073_CustomLayoutPanel : Panel
{
    /// <summary>Given. Do not change. Offset between one child and the next, on both axes.</summary>
    public static readonly StyledProperty<double> StepProperty =
        AvaloniaProperty.Register<Ex073_CustomLayoutPanel, double>(
            nameof(Step), defaultValue: 5.0);

    static Ex073_CustomLayoutPanel() => AffectsMeasure<Ex073_CustomLayoutPanel>(StepProperty);

    public double Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) =>
        throw new NotImplementedException(
            "TODO: Ex073 - measure every child against availableSize, then return the " +
            "union of the staircase: for child i at offset i * Step, the corner it " +
            "reaches is i * Step plus its own DesiredSize. An empty panel wants 0x0");

    protected override Size ArrangeOverride(Size finalSize) =>
        throw new NotImplementedException(
            "TODO: Ex073 - Arrange child i into a Rect at (i * Step, i * Step) sized " +
            "to that child's DesiredSize, and return finalSize");
}
