// Exercise 073 - Invalidation Tracking (advanced).
// Goal:   Know exactly what triggers a measure pass and what only triggers an arrange.
// Drills: InvalidateMeasure against InvalidateArrange, a property that affects one but not
//         the other, and counting the passes to prove it.
// Passes: dotnet test --filter FullyQualifiedName~Ex073_
//
// Marking measure dirty always costs an arrange too - the arrange has to follow the new
// sizes. The reverse is not true, and that is where the win is: a property that only moves
// children (an offset, an alignment) should invalidate arrange alone, and a control that
// calls InvalidateMeasure for it re-measures the whole subtree on every animation frame.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A panel that counts its own passes and exposes two properties: one that changes how big
/// it wants to be, and one that only changes where it puts its children.
/// </summary>
public partial class Ex073_InvalidationTracking : Panel
{
    /// <summary>How many times MeasureOverride has run.</summary>
    public int MeasurePasses { get; private set; }

    /// <summary>How many times ArrangeOverride has run.</summary>
    public int ArrangePasses { get; private set; }

    /// <summary>
    /// Extra space around the children. Changes how big this panel wants to be, so it
    /// affects both passes.
    /// </summary>
    public static readonly DependencyProperty GutterProperty =
        DependencyProperty.Register(
            nameof(Gutter),
            typeof(double),
            typeof(Ex073_InvalidationTracking),
            new PropertyMetadata(0d, OnGutterChanged));

    /// <summary>
    /// How far the children are pushed to the right within the space the panel already has.
    /// Cannot change the panel's desired size, so it affects the arrange pass only.
    /// </summary>
    public static readonly DependencyProperty ShiftProperty =
        DependencyProperty.Register(
            nameof(Shift),
            typeof(double),
            typeof(Ex073_InvalidationTracking),
            new PropertyMetadata(0d, OnShiftChanged));

    public double Gutter
    {
        get => (double)GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    public double Shift
    {
        get => (double)GetValue(ShiftProperty);
        set => SetValue(ShiftProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        MeasurePasses++;

        var height = 0d;
        var width = 0d;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(width + (2 * Gutter), height + (2 * Gutter));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ArrangePasses++;

        var y = Gutter;

        foreach (var child in Children)
        {
            child.Arrange(new Rect(Gutter + Shift, y, child.DesiredSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        return finalSize;
    }

    // Measure only: marking measure dirty makes the arrange follow it anyway, so
    // invalidating both here would be redundant rather than wrong.
    private static void OnGutterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex073_InvalidationTracking)sender).InvalidateMeasure();

    // Arrange only. The shift cannot change any size, so a measure pass would re-run the
    // whole subtree for nothing - once per frame, if this is being animated.
    private static void OnShiftChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex073_InvalidationTracking)sender).InvalidateArrange();
}
