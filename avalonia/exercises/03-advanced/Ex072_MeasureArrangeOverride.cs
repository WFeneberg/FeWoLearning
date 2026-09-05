using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 072 - MeasureArrangeOverride (advanced).
/// Goal:   Implement the two halves of the layout protocol on a control that keeps
///         a fixed aspect ratio: MeasureOverride answers how big it would like to
///         be given at most this much, ArrangeOverride commits to what it actually
///         got.
/// Drills: MeasureOverride, ArrangeOverride, DesiredSize, and the infinite
///         constraint that every naive implementation forgets.
/// Passes: dotnet test --filter FullyQualifiedName~Ex072_
///
/// The infinity case is the whole difficulty. A parent that does not constrain an
/// axis - a StackPanel on its stacking axis, a ScrollViewer on both - passes
/// double.PositiveInfinity, and a DesiredSize computed from it comes back infinite
/// too, which corrupts the parent's own arithmetic. Layout must never return an
/// infinite DesiredSize.
///
/// Exceptions thrown from either override surface at Show(), synchronously during
/// the layout pass. Measured, and unlike Render, whose exceptions only appear when
/// the dispatcher is drained.
public class Ex072_MeasureArrangeOverride : Control
{
    /// <summary>Given. Do not change. Width divided by height.</summary>
    public static readonly StyledProperty<double> AspectRatioProperty =
        AvaloniaProperty.Register<Ex072_MeasureArrangeOverride, double>(
            nameof(AspectRatio), defaultValue: 2.0);

    /// <summary>Given. Do not change. The width to fall back on when nothing constrains us.</summary>
    public const double UnconstrainedWidth = 100.0;

    public double AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    /// <summary>
    /// The largest size at AspectRatio that fits inside availableSize.
    ///
    /// The contract, which the test pins down case by case:
    ///   - both axes finite: the biggest box at this ratio that fits, so it touches
    ///     one edge and leaves slack on the other. 200x200 at ratio 2 is 200x100,
    ///     and 200x40 is 80x40;
    ///   - width infinite, height finite: take the height and derive the width;
    ///   - height infinite, width finite: take the width and derive the height;
    ///   - both infinite: UnconstrainedWidth by UnconstrainedWidth over AspectRatio.
    /// </summary>
    protected override Size MeasureOverride(Size availableSize) =>
        throw new NotImplementedException(
            "TODO: Ex072 - return the largest size at AspectRatio fitting " +
            "availableSize, treating each infinite axis as unconstrained per the " +
            "contract above. Never return an infinite DesiredSize");

    protected override Size ArrangeOverride(Size finalSize) =>
        throw new NotImplementedException(
            "TODO: Ex072 - the parent has decided; report finalSize as the size " +
            "actually used, which is what fixes Bounds");
}
