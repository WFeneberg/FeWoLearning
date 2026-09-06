using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 071 - CustomControlRender (advanced).
/// Goal:   Draw a sparkline by overriding Control.Render, and get the coordinate
///         maths right: N values become a polyline across the control's box, with
///         the smallest value on the BOTTOM edge and the largest on the top,
///         because screen y grows downwards.
/// Drills: Control.Render(DrawingContext), StreamGeometry.Open and the figure API,
///         mapping data space to control space, surviving degenerate input.
/// Passes: dotnet test --filter FullyQualifiedName~Ex071_
///
/// WHAT IS GRADED, AND WHAT CANNOT BE. The mapping is graded exactly, through
/// BuildPoints. The individual DRAW CALLS are not graded, and cannot be - two
/// measurements rule out the obvious approaches:
///   - DrawingContext has a PRIVATE constructor, so no recording double can be
///     derived from it;
///   - the render data a real context records is entirely internal
///     (RenderDataDrawingContext, CompositionRenderData, and Visual's own
///     CompositionVisual are all non-public).
/// Nor can the geometry be inspected instead - a StreamGeometry is write-only by
/// design, and FillContains is not usable here either (see ex074's header for that
/// measurement). Hence BuildPoints: the arithmetic worth teaching is separated out
/// where a test can actually reach it.
///
/// PIXELS, HOWEVER, ARE READABLE. The harness builds its app with .UseSkia() and
/// UseHeadlessDrawing turned off, so TopLevel.CaptureRenderedFrame returns a real
/// frame - that is ex098's whole subject. This exercise still grades the maths
/// rather than the picture, on purpose: a two-unit anti-aliased polyline is a poor
/// thing to sample, because whether any given pixel is black depends on coverage
/// rather than on the code being right. ex098 asserts pixels where the shapes are
/// solid blocks and the answer is unambiguous.
public class Ex071_CustomControlRender : Control
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<IReadOnlyList<double>> ValuesProperty =
        AvaloniaProperty.Register<Ex071_CustomControlRender, IReadOnlyList<double>>(
            nameof(Values), defaultValue: []);

    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Ex071_CustomControlRender, double>(
            nameof(StrokeThickness), defaultValue: 2.0);

    static Ex071_CustomControlRender() => AffectsRender<Ex071_CustomControlRender>(ValuesProperty);

    public IReadOnlyList<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Values mapped into a box of <paramref name="size"/>, in order.
    ///
    /// Point i sits at x = i * size.Width / (N - 1), so the first is on the left
    /// edge and the last on the right. Its y is the value normalised so the
    /// MINIMUM lands on size.Height and the MAXIMUM on 0.
    ///
    /// Three degenerate cases the test pins down, all of which divide by zero in
    /// the obvious implementation:
    ///   - no values at all: an empty list;
    ///   - exactly one value: one point, horizontally centred, at mid height;
    ///   - every value identical: a flat line across the vertical middle.
    /// </summary>
    public IReadOnlyList<Point> BuildPoints(Size size) =>
        throw new NotImplementedException(
            "TODO: Ex071 - map each value to a Point per the contract above, and " +
            "guard the three degenerate cases before dividing by anything");

    public override void Render(DrawingContext context) =>
        throw new NotImplementedException(
            "TODO: Ex071 - take BuildPoints(Bounds.Size), open a StreamGeometry, " +
            "BeginFigure at the first point with isFilled false, LineTo the rest, " +
            "EndFigure(false), and stroke it with a Pen over Brushes.Black at " +
            "StrokeThickness. Must not throw for any Values, including an empty " +
            "list, because Render runs on the render path where an exception costs " +
            "you the frame rather than a test");
}
