// Exercise 084 - Render Transforms (advanced).
// Goal:   Compute where a transform actually puts things.
// Drills: RotateTransform/ScaleTransform/TranslateTransform, TransformPoint against
//         TransformBounds, and the order a TransformGroup applies its children in.
// Passes: dotnet test --filter FullyQualifiedName~Ex084_
//
// A render transform changes what is drawn, not what was laid out - the element keeps the
// slot it was arranged into, which is why a rotated element overlaps its neighbours instead
// of pushing them aside.
//
// TransformBounds is not TransformPoint applied to a corner: it returns the axis-aligned
// box that contains the transformed shape, so a 45-degree rotation makes it bigger than the
// original in both directions. Confusing the two is how hit-testing ends up subtly wrong.

using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

public static class Ex084_RenderTransforms
{
    /// <summary>A rotation about the origin, in degrees, clockwise.</summary>
    public static Transform CreateRotation(double degrees) =>
        throw new NotImplementedException("TODO: Ex084 - build the rotation");

    /// <summary>A scale about the origin.</summary>
    public static Transform CreateScale(double x, double y) =>
        throw new NotImplementedException("TODO: Ex084 - build the scale");

    /// <summary>
    /// A group that scales first and then rotates - which is the order the children appear
    /// in, not the reverse.
    /// </summary>
    public static Transform CreateScaleThenRotate(double scale, double degrees) =>
        throw new NotImplementedException("TODO: Ex084 - compose the two transforms");

    /// <summary>Where <paramref name="point"/> ends up under <paramref name="transform"/>.</summary>
    public static Point Map(Transform transform, Point point) =>
        throw new NotImplementedException("TODO: Ex084 - map the point");

    /// <summary>
    /// The axis-aligned box containing <paramref name="rect"/> after
    /// <paramref name="transform"/>.
    /// </summary>
    public static Rect MapBounds(Transform transform, Rect rect) =>
        throw new NotImplementedException("TODO: Ex084 - map the bounds");
}
