// Exercise 032 - Value Precedence (beginner).
// Goal:   Find out where the value a property currently reports actually came from.
// Drills: ReadLocalValue against DependencyProperty.UnsetValue, the local-beats-style-beats-
//         default order, and Style.Setters as the middle rung.
// Passes: dotnet test --filter FullyQualifiedName~Ex032_
//
// "The style is not applying" is almost always this: something set the property locally,
// and a local value outranks every style. GetValue cannot tell you - it returns the
// effective value with no provenance. ReadLocalValue can.

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
    public static string DescribeSource(FrameworkElement element, DependencyProperty property) =>
        // TODO: ask for the local value first - UnsetValue means "nobody set one here".
        // Then look through the element's Style (and the styles it is based on) for a
        // setter naming this property. Otherwise it is the registered default.
        throw new NotImplementedException("TODO: Ex032 - describe where the value comes from");
}
