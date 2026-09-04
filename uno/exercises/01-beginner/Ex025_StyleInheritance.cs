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
    public static Style CreateBaseStyle() =>
        throw new NotImplementedException("TODO: Ex025 - build the base style");

    /// <summary>
    /// A style based on <paramref name="baseStyle"/> that keeps the inherited Width and
    /// overrides Height to 80. It must not repeat the Width setter.
    /// </summary>
    public static Style CreateWideStyle(Style baseStyle) =>
        // TODO: build the derived style, point BasedOn at the base, and add only the one
        // setter that differs.
        throw new NotImplementedException("TODO: Ex025 - derive a style from the base");

    /// <summary>
    /// A Border with <paramref name="style"/> applied through
    /// <see cref="FrameworkElement.Style"/>.
    /// </summary>
    public static Border CreateStyled(Style style) =>
        throw new NotImplementedException("TODO: Ex025 - apply the style to a border");
}
