// Exercise 024 - Implicit Style (beginner).
// Goal:   Style every element of a type in a scope, without touching any of them.
// Drills: Style/Setter built in code, the type as the resource key that makes a style
//         implicit, and the fact that a local value still beats a style setter.
// Passes: dotnet test --filter FullyQualifiedName~Ex024_
//
// In markup a Style without x:Key is implicit. In code that is not magic either: the key
// is the target type itself, which is why the lookup can find it from any Border.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex024_ImplicitStyle
{
    /// <summary>
    /// A Style for <see cref="Border"/> setting Width to 77 and Height to 33.
    /// </summary>
    public static Style CreateBorderStyle() =>
        // TODO: build the Style with its TargetType and two Setters.
        throw new NotImplementedException("TODO: Ex024 - build the border style");

    /// <summary>
    /// A StackPanel that carries <see cref="CreateBorderStyle"/> as an *implicit* style, so
    /// every Border below it picks it up, and holds the given children.
    /// </summary>
    public static StackPanel CreateStyledScope(params FrameworkElement[] children) =>
        // TODO: create the panel, register the style in its Resources under the key that
        // makes it implicit, and add the children.
        throw new NotImplementedException("TODO: Ex024 - register the implicit style on a scope");
}
