// Exercise 003 - Attached Property (beginner).
// Goal:   Store a value on somebody else's element - the mechanism behind Grid.Row.
// Drills: DependencyProperty.RegisterAttached, the static GetX/SetX accessor pair that
//         XAML looks for, and where the value actually lives (on the target, not in a
//         side table owned by this class).
// Passes: dotnet test --filter FullyQualifiedName~Ex003_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>
/// A layout slot that any element can carry, whether or not its parent understands it.
/// </summary>
public static class Ex003_AttachedProperty
{
    // RegisterAttached, not Register: the owner named here is only the *declaring* type.
    // The value is stored in the property store of whatever element it is set on, which
    // is why this class needs no dictionary and no lifetime management.
    public static readonly DependencyProperty SlotProperty =
        DependencyProperty.RegisterAttached(
            "Slot",
            typeof(int),
            typeof(Ex003_AttachedProperty),
            new PropertyMetadata(-1));

    public static int GetSlot(DependencyObject element) => (int)element.GetValue(SlotProperty);

    public static void SetSlot(DependencyObject element, int value) => element.SetValue(SlotProperty, value);
}
