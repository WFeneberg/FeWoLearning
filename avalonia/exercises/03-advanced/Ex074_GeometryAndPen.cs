using Avalonia;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 074 - GeometryAndPen (advanced).
/// Goal:   Describe a chevron as a PathGeometry, describe how it should be stroked
///         with a Pen, and see that a pen changes an outline's footprint - a
///         stroke straddles the path, so it sticks out by half its thickness on
///         every side.
/// Drills: PathGeometry, PathFigure, LineSegment, FillRule, Geometry.Bounds versus
///         GetRenderBounds, Pen thickness, caps, joins and dashes.
/// Passes: dotnet test --filter FullyQualifiedName~Ex074_
///
/// WHY PathGeometry AND NOT StreamGeometry, which is the type most tutorials
/// reach for. A StreamGeometry is write-only by design: you push segments into a
/// sink and there is no way to read them back, so a test can learn nothing about
/// the shape beyond its Bounds - which a plain rectangle would satisfy just as
/// well. A PathGeometry keeps its Figures and Segments as ordinary objects, so the
/// vertices can be asserted one by one. Use StreamGeometry when you are drawing
/// and never inspecting, as ex071's Render does; use PathGeometry when the shape
/// itself is data.
///
/// TWO MEASURED LIMITS BEHIND THAT CHOICE, both worth knowing before writing any
/// geometry test in this harness.
///
/// FillContains cannot be trusted here. Sampled on a grid, a SOLID arrow reported
/// its own centre row as hollow and its left edge as outside; a self-intersecting
/// star reported the centre as filled under EvenOdd and NonZero alike, when
/// distinguishing exactly that is the entire purpose of the fill rule. So the fill
/// rule below is graded as the property it is, not as a hole in a shape.
///
/// StrokeContains is worse: it returned false for a point plainly inside a 10 px
/// stroke down the middle of a horizontal line. Do not build anything on it.
/// GetRenderBounds, by contrast, is exact - measured across thicknesses 1, 4 and
/// 10, it inflates the bounds by precisely half the thickness on every side.
public static class Ex074_GeometryAndPen
{
    /// <summary>
    /// A right-pointing chevron as ONE closed, filled figure: StartPoint at (0, 0)
    /// followed by five LineSegments through, in this order,
    ///
    ///   (width - inset, 0), (width, height / 2),
    ///   (width - inset, height), (0, height), (inset, height / 2)
    ///
    /// The last point is what makes it a chevron rather than an arrow: it notches
    /// the left edge inwards, so the shape reads as a thick "greater than" sign.
    /// </summary>
    public static PathGeometry BuildChevron(double width, double height, double inset) =>
        throw new NotImplementedException(
            "TODO: Ex074 - a PathGeometry with one PathFigure whose StartPoint is " +
            "(0,0), IsClosed and IsFilled both true, and whose Segments are the five " +
            "LineSegments listed above in order");

    /// <summary>
    /// The pen the chevron is meant to be stroked with: 4 units thick, round caps,
    /// round joins, and a dash pattern of 2 on and 2 off.
    /// </summary>
    public static Pen BuildPen() =>
        throw new NotImplementedException(
            "TODO: Ex074 - a Pen over Brushes.Black with Thickness 4, LineCap and " +
            "LineJoin both Round, and a DashStyle whose Dashes are 2 and 2");

    /// <summary>
    /// A square ring: an outer square from (0,0) to (40,40) and an inner one from
    /// (10,10) to (30,30), as two figures of the SAME geometry, under the given
    /// fill rule.
    ///
    /// With EvenOdd the middle would be a hole; with NonZero it depends on the two
    /// winding directions. This harness can show neither - see the class header -
    /// so the test asserts the rule and the figures, not the hole.
    /// </summary>
    public static PathGeometry BuildRing(FillRule rule) =>
        throw new NotImplementedException(
            "TODO: Ex074 - a PathGeometry with FillRule set to rule and two closed, " +
            "filled PathFigures of LineSegments: the outer square 0,0 to 40,40 and " +
            "the inner square 10,10 to 30,30");
}
