// Exercise 024 - Implicit Style (beginner).
// Goal:   Style every element of a type in a scope, without touching any of them.
// Drills: Style/Setter built in code, the type as the resource key that makes a style
//         implicit, and the fact that a local value still beats a style setter.
// Passes: dotnet test --filter FullyQualifiedName~Ex024_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex024_ImplicitStyle
{
    /// <summary>
    /// A Style for <see cref="Border"/> setting Width to 77 and Height to 33.
    /// </summary>
    public static Style CreateBorderStyle() => new(typeof(Border))
    {
        // A Setter names the dependency property, not a CLR property - which is why only
        // dependency properties can be styled at all.
        Setters =
        {
            new Setter(FrameworkElement.WidthProperty, 77d),
            new Setter(FrameworkElement.HeightProperty, 33d),
        },
    };

    /// <summary>
    /// A StackPanel that carries <see cref="CreateBorderStyle"/> as an *implicit* style, so
    /// every Border below it picks it up, and holds the given children.
    /// </summary>
    public static StackPanel CreateStyledScope(params FrameworkElement[] children)
    {
        var panel = new StackPanel();

        // The target type *is* the key. That is the whole of "implicit": a Border looks up
        // typeof(Border) in the usual upward resource walk, and finds this.
        panel.Resources[typeof(Border)] = CreateBorderStyle();

        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }
}
