// Exercise 025 - Style Inheritance (beginner).
// Goal:   Extend a style instead of copying it.
// Drills: Style.BasedOn, a derived setter overriding an inherited one, and the fact that a
//         style is sealed once it is used - so the chain is built before anything applies.
// Passes: dotnet test --filter FullyQualifiedName~Ex025_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex025_StyleInheritance
{
    /// <summary>
    /// The base style for a <see cref="Border"/>: Width 100, Height 40.
    /// </summary>
    public static Style CreateBaseStyle() => new(typeof(Border))
    {
        Setters =
        {
            new Setter(FrameworkElement.WidthProperty, 100d),
            new Setter(FrameworkElement.HeightProperty, 40d),
        },
    };

    /// <summary>
    /// A style based on <paramref name="baseStyle"/> that keeps the inherited Width and
    /// overrides Height to 80. It must not repeat the Width setter.
    /// </summary>
    public static Style CreateWideStyle(Style baseStyle) => new(typeof(Border))
    {
        BasedOn = baseStyle,

        // Only the difference. The lookup walks the BasedOn chain and the nearest setter
        // for a property wins, so this Height shadows the base's without removing it.
        Setters = { new Setter(FrameworkElement.HeightProperty, 80d) },
    };

    /// <summary>
    /// A Border with <paramref name="style"/> applied through
    /// <see cref="FrameworkElement.Style"/>.
    /// </summary>
    public static Border CreateStyled(Style style) => new() { Style = style };
}
