// Exercise 032 - Value Precedence (beginner).
// Goal:   Find out where the value a property currently reports actually came from.
// Drills: ReadLocalValue against DependencyProperty.UnsetValue, the local-beats-style-beats-
//         default order, and Style.Setters as the middle rung.
// Passes: dotnet test --filter FullyQualifiedName~Ex032_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex032_ValuePrecedence
{
    /// <summary>
    /// Where the effective value of <paramref name="property"/> on
    /// <paramref name="element"/> comes from: <c>"local"</c> when it was set on the element
    /// itself, <c>"style"</c> when a setter of the element's Style provides it, and
    /// <c>"default"</c> when neither does.
    /// </summary>
    public static string DescribeSource(FrameworkElement element, DependencyProperty property)
    {
        // ReadLocalValue is the only API that distinguishes "set to X" from "happens to be
        // X". GetValue would answer 50 for both a local 50 and a styled 50.
        if (element.ReadLocalValue(property) != DependencyProperty.UnsetValue)
        {
            return "local";
        }

        return HasSetterFor(element.Style, property) ? "style" : "default";
    }

    /// <summary>
    /// Walks the BasedOn chain: the element only knows its own Style, and the setter that
    /// wins may be several levels up.
    /// </summary>
    private static bool HasSetterFor(Style? style, DependencyProperty property)
    {
        for (var current = style; current is not null; current = current.BasedOn)
        {
            foreach (var setterBase in current.Setters)
            {
                if (setterBase is Setter setter && setter.Property == property)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
