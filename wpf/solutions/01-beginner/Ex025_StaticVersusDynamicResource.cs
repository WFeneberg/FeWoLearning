// Exercise 025 - StaticResource vs DynamicResource, in code (beginner). REFERENCE SOLUTION.
// Goal:   See the one real difference between the two markup extensions - a one-time
//         lookup versus a reference that keeps following its key - through their actual
//         code-only equivalents. Neither StaticResource nor DynamicResource exists as such
//         in code; this tier has no XAML, so the honest contrast is between resolving a
//         resource once and writing the literal result, versus FrameworkElement.
//         SetResourceReference, which is what actually keeps following the key when the
//         resource is later replaced. Map this onto the markup extensions once you reach
//         XAML: {StaticResource key} behaves like ApplyOnce, {DynamicResource key} like
//         ApplyFollowing.
// Drills: FrameworkElement.FindResource (a one-time, current-value lookup) plus a plain
//         SetValue - the code equivalent of {StaticResource key} - versus
//         FrameworkElement.SetResourceReference(property, key) - the code equivalent of
//         {DynamicResource key} - which keeps tracking the key for as long as the
//         reference lives.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex025_StaticVersusDynamicResource
{
    /// <summary>
    /// The code equivalent of {StaticResource key}: resolves <paramref name="key"/> once,
    /// right now, and writes the literal result into <paramref name="property"/>. A later
    /// change to the resource behind <paramref name="key"/> is never seen again.
    /// </summary>
    public static void ApplyOnce(FrameworkElement target, DependencyProperty property, object key)
    {
        target.SetValue(property, target.FindResource(key));
    }

    /// <summary>
    /// The code equivalent of {DynamicResource key}: keeps following <paramref name="key"/>
    /// for as long as the reference lives, picking up later swaps of the resource behind it.
    /// </summary>
    public static void ApplyFollowing(FrameworkElement target, DependencyProperty property, object key)
    {
        target.SetResourceReference(property, key);
    }
}
