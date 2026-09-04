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
    // TODO: register an attached property: name "Slot", type int, owner
    // Ex003_AttachedProperty, default value -1. Expose it as a public static readonly
    // field called SlotProperty.
    //
    // The accessor names are a contract, not a style choice: XAML resolves
    // <Border local:Ex003_AttachedProperty.Slot="2" /> by looking for GetSlot/SetSlot.

    public static int GetSlot(DependencyObject element) =>
        throw new NotImplementedException("TODO: Ex003 - read Slot off the element");

    public static void SetSlot(DependencyObject element, int value) =>
        throw new NotImplementedException("TODO: Ex003 - write Slot onto the element");
}
