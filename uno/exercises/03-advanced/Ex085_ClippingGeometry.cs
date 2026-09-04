// Exercise 085 - Clipping Geometry (advanced).
// Goal:   Clip an element and reason about the geometry doing it.
// Drills: UIElement.Clip taking a RectangleGeometry and nothing else, Geometry.Bounds, and
//         the fact that a clip changes neither the layout nor the measured size.
// Passes: dotnet test --filter FullyQualifiedName~Ex085_
//
// UIElement.Clip is typed as RectangleGeometry, not Geometry - so the rounded or arbitrary
// clip everybody reaches for first is not available here at all, and the answer is a
// composition brush or a Border with a corner radius instead.
//
// A clip is a paint-time operation: the element still measures and arranges at its full
// size, so clipping something to nothing leaves a full-size hole in the layout. That is the
// difference to Visibility.Collapsed, and it is why "it is clipped" and "it is gone" are
// different bugs.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

public static class Ex085_ClippingGeometry
{
    /// <summary>A rectangle geometry over <paramref name="rect"/>.</summary>
    public static RectangleGeometry CreateClip(Rect rect) =>
        throw new NotImplementedException("TODO: Ex085 - build the clip geometry");

    /// <summary>
    /// Clips <paramref name="element"/> to <paramref name="rect"/> and returns it.
    /// </summary>
    public static T Clip<T>(T element, Rect rect)
        where T : UIElement =>
        throw new NotImplementedException("TODO: Ex085 - apply the clip");

    /// <summary>Removes any clip from <paramref name="element"/>.</summary>
    public static void Unclip(UIElement element) =>
        throw new NotImplementedException("TODO: Ex085 - remove the clip");

    /// <summary>
    /// The part of <paramref name="content"/> that survives a clip to
    /// <paramref name="clip"/> - the intersection, or an empty rect when they do not meet.
    /// </summary>
    public static Rect VisiblePart(Rect content, Rect clip) =>
        // TODO: intersect the two rects. Rect has an Intersect method that mutates in
        // place and yields an empty rect when there is no overlap - which is not the same
        // as a zero-sized rect at the origin.
        throw new NotImplementedException("TODO: Ex085 - intersect the content with the clip");
}
