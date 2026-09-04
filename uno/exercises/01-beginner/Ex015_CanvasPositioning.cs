// Exercise 015 - Canvas Positioning (beginner).
// Goal:   Place children at absolute coordinates, outside the layout negotiation.
// Drills: Canvas.Left/Canvas.Top/Canvas.ZIndex as attached properties, the fact that a
//         Canvas measures its children with infinite space and itself as nothing, and that
//         ZIndex changes paint order without touching the Children order.
// Passes: dotnet test --filter FullyQualifiedName~Ex015_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex015_CanvasPositioning
{
    /// <summary>
    /// A Canvas holding both children, <paramref name="back"/> added first:
    /// <list type="bullet">
    ///   <item><paramref name="back"/> at (10, 20) with a ZIndex of 1,</item>
    ///   <item><paramref name="front"/> at (120, 40) with a ZIndex of 0.</item>
    /// </list>
    /// Note the names are about painting, not about the order they were added: "front" goes
    /// in second and still ends up underneath.
    /// </summary>
    public static Canvas CreateScene(FrameworkElement back, FrameworkElement front) =>
        // TODO: create the Canvas, add both children in the documented order, and set
        // their positions and ZIndex.
        throw new NotImplementedException("TODO: Ex015 - lay out the canvas scene");
}
