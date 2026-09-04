// Exercise 043 - Inherited Context (intermediate).
// Goal:   Let a subtree read a value an ancestor declared, without a static or a singleton.
// Drills: walking up with VisualTreeHelper.GetParent, an attached property as the carrier,
//         and the nearest declaration winning.
// Passes: dotnet test --filter FullyQualifiedName~Ex043_
//
// WPF had FrameworkPropertyMetadata.Inherits; WinUI does not expose property inheritance at
// all - DataContext and RequestedTheme are inherited by the framework itself, and there is
// no public way to add a third. So "ambient" values are found by asking upwards, which is
// also what {StaticResource} does. Doing it by hand is the way to understand both.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex043_InheritedContext
{
    /// <summary>
    /// The carrier: set it on any element to declare a density for that element and
    /// everything below it. Given, so the exercise is about the lookup.
    /// </summary>
    public static readonly DependencyProperty DensityProperty =
        DependencyProperty.RegisterAttached(
            "Density",
            typeof(int),
            typeof(Ex043_InheritedContext),
            new PropertyMetadata(0));

    public static int GetDensity(DependencyObject element) => (int)element.GetValue(DensityProperty);

    public static void SetDensity(DependencyObject element, int value) => element.SetValue(DensityProperty, value);

    /// <summary>
    /// The density in effect for <paramref name="element"/>: the value declared on the
    /// element itself, or on the nearest ancestor that declared one, or
    /// <paramref name="fallback"/> when nobody did.
    /// </summary>
    /// <remarks>
    /// "Declared" means locally set. An element whose Density simply reads 0 because that is
    /// the registered default has not declared anything, and the walk must continue past it.
    /// </remarks>
    public static int EffectiveDensity(DependencyObject element, int fallback)
    {
        for (DependencyObject? current = element; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            // ReadLocalValue, not GetValue: GetValue answers 0 both for "somebody said 0"
            // and for "nobody said anything", and stopping on the second one would end the
            // walk at the first element that never took part.
            if (current.ReadLocalValue(DensityProperty) is int declared)
            {
                return declared;
            }
        }

        return fallback;
    }
}
